import {
	Dropdown,
	Field,
	Option,
	Spinner,
	makeStyles,
} from "@fluentui/react-components";
import type React from "react";
import { useEffect, useMemo, useState } from "react";
import { DataSourceType } from "../../api/datasource/models";
import { useDataSourcePicker } from "./datasource/useDataSourcePicker";
import type { TagPickerOption } from "./TagPicker";

const useStyles = makeStyles({
	dropdown: {
		minWidth: "200px",
	},
});

export interface TeamPickerProps {
	sbuId?: string | null;
	sbuGid?: string | null;
	value: string | null;
	onChange: (value: string | null, option?: TagPickerOption | null) => void;
	label?: string;
	placeholder?: string;
	required?: boolean;
	error?: string;
	disabled?: boolean;
	useGid?: boolean;
}

const TeamPicker: React.FC<TeamPickerProps> = ({
	sbuId,
	sbuGid,
	value,
	onChange,
	label = "Zespół",
	placeholder = "Wybierz zespół...",
	required = false,
	error,
	disabled = false,
	useGid = true,
}) => {
	const styles = useStyles();
	const [options, setOptions] = useState<TagPickerOption[]>([]);
	const [loading, setLoading] = useState(false);

	const filterId = sbuId ?? sbuGid;

	const contextFilters = useMemo(
		() => (filterId ? { SbuId: filterId } : undefined),
		[filterId],
	);

	const { loadOptions } = useDataSourcePicker({
		dataSourceType: DataSourceType.Teams,
		contextFilters,
		useGid,
	});

	useEffect(() => {
		if (!filterId) {
			setOptions([]);
			return;
		}

		const fetchOptions = async () => {
			setLoading(true);
			try {
				const items = await loadOptions("", undefined, 100);
				setOptions(items);
			} catch (err) {
				console.error("Failed to load Team options:", err);
			} finally {
				setLoading(false);
			}
		};
		fetchOptions();
	}, [loadOptions, filterId]);

	const selectedOption = options.find((o) => o.id === value);
	const isValueValid = !value || options.some((o) => o.id === value);

	return (
		<Field
			label={required ? `${label} *` : label}
			validationState={error ? "error" : "none"}
			validationMessage={error}
		>
			<Dropdown
				className={styles.dropdown}
				placeholder={
					!filterId
						? "Najpierw wybierz Strukture"
						: loading
							? "Ładowanie..."
							: placeholder
				}
				value={isValueValid ? (selectedOption?.name ?? "") : ""}
				selectedOptions={isValueValid && value ? [value] : []}
				onOptionSelect={(_, data) => {
					const newValue = data.optionValue as string | undefined;
					const selectedOpt = options.find((o) => o.id === newValue);
					onChange(newValue ?? null, selectedOpt ?? null);
				}}
				disabled={disabled || !filterId || loading}
			>
				{loading ? (
					<Option key="loading" value="" text="Ładowanie..." disabled>
						<Spinner size="tiny" /> Ładowanie...
					</Option>
				) : (
					options.map((option) => (
						<Option key={option.id} value={option.id} text={option.name}>
							{option.name}
						</Option>
					))
				)}
			</Dropdown>
		</Field>
	);
};

export default TeamPicker;
