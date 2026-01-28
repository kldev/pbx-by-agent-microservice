import {
	Dropdown,
	Field,
	Option,
	Spinner,
	makeStyles,
} from "@fluentui/react-components";
import type React from "react";
import { useEffect, useState } from "react";
import { DataSourceType } from "../../api/datasource/models";
import { useDataSourcePicker } from "./datasource/useDataSourcePicker";
import type { TagPickerOption } from "./TagPicker";

const useStyles = makeStyles({
	dropdown: {
		minWidth: "200px",
	},
});

export interface CertificatePickerProps {
	value: string | null;
	onChange: (
		value: string | null,
		option: { id: string; name: string } | null,
	) => void;
	label?: string;
	placeholder?: string;
	required?: boolean;
	error?: string;
	disabled?: boolean;
}

export const CertificatePicker: React.FC<CertificatePickerProps> = ({
	value,
	onChange,
	label = "Certyfikat",
	placeholder = "Wybierz certyfikat...",
	required = false,
	error,
	disabled = false,
}) => {
	const styles = useStyles();
	const [options, setOptions] = useState<TagPickerOption[]>([]);
	const [loading, setLoading] = useState(true);

	const { loadOptions } = useDataSourcePicker({
		dataSourceType: DataSourceType.Certificates,
	});

	useEffect(() => {
		const fetchOptions = async () => {
			setLoading(true);
			try {
				const items = await loadOptions("", undefined, 100);
				setOptions(items);
			} catch (err) {
				console.error("Failed to load Certificate options:", err);
			} finally {
				setLoading(false);
			}
		};
		fetchOptions();
	}, [loadOptions]);

	const selectedCert = options.find((c) => c.id === value);

	return (
		<Field
			label={required ? `${label} *` : label}
			validationState={error ? "error" : "none"}
			validationMessage={error}
		>
			<Dropdown
				className={styles.dropdown}
				placeholder={loading ? "Ładowanie..." : placeholder}
				value={selectedCert?.name ?? ""}
				selectedOptions={value ? [value] : []}
				onOptionSelect={(_, data) => {
					const id = data.optionValue as string | undefined;
					const cert = options.find((c) => c.id === id);
					onChange(id ?? null, cert ? { id: cert.id, name: cert.name } : null);
				}}
				disabled={disabled || loading}
			>
				{loading ? (
					<Option key="loading" value="" text="Ładowanie..." disabled>
						<Spinner size="tiny" /> Ładowanie...
					</Option>
				) : (
					options.map((cert) => (
						<Option key={cert.id} value={cert.id} text={cert.name}>
							{cert.name}
							{cert.secondary && (
								<span style={{ color: "gray", marginLeft: 8, fontSize: 12 }}>
									({cert.secondary})
								</span>
							)}
						</Option>
					))
				)}
			</Dropdown>
		</Field>
	);
};

export default CertificatePicker;
