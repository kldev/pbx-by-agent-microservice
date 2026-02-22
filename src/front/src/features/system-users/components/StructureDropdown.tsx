import {
	Dropdown,
	Option,
	Spinner,
	Text,
	makeStyles,
	tokens,
} from "@fluentui/react-components";
import type {
	OptionOnSelectData,
	SelectionEvents,
} from "@fluentui/react-components";
import type React from "react";
import { useEffect } from "react";
import { useGetAllStructure } from "../../../api/identity/endpoints/structure/structure";

const useStyles = makeStyles({
	errorText: {
		color: tokens.colorPaletteRedForeground1,
		fontSize: tokens.fontSizeBase200,
	},
});

export interface StructureDropdownProps {
	value?: number | null;
	onChange: (structureId: number | null) => void;
	disabled?: boolean;
	placeholder?: string;
}

const StructureDropdown: React.FC<StructureDropdownProps> = ({
	value,
	onChange,
	disabled = false,
	placeholder = "Wybierz strukturę",
}) => {
	const styles = useStyles();

	const { mutate, data, isPending, isError } = useGetAllStructure();

	useEffect(() => {
		mutate({ data: { isActive: true } });
	}, [mutate]);

	const structures = data?.status === 200 ? data.data : [];

	const selectedStructure = structures.find((s) => s.id === value);
	const selectedValue = selectedStructure?.name ?? "";

	const handleSelect = (
		_ev: SelectionEvents,
		selectData: OptionOnSelectData,
	) => {
		const id = selectData.optionValue ? Number(selectData.optionValue) : null;
		onChange(id);
	};

	if (isPending) {
		return <Spinner size="tiny" />;
	}

	if (isError) {
		return <Text className={styles.errorText}>Błąd ładowania struktur</Text>;
	}

	return (
		<Dropdown
			value={selectedValue}
			selectedOptions={value != null ? [String(value)] : []}
			onOptionSelect={handleSelect}
			placeholder={placeholder}
			disabled={disabled}
		>
			{structures.map((structure) => (
				<Option key={structure.id} value={String(structure.id)}>
					{structure.name ?? structure.code ?? String(structure.id)}
				</Option>
			))}
		</Dropdown>
	);
};

export default StructureDropdown;
