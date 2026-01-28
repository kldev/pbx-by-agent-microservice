import {
	Dropdown,
	Field,
	Option,
	makeStyles,
} from "@fluentui/react-components";
import type React from "react";
import type { EnumOption } from "../../constants/enumLabels";

const useStyles = makeStyles({
	dropdown: {
		minWidth: "200px",
	},
});

export interface EnumPickerProps<T extends string> {
	label: string;
	options: EnumOption<T>[];
	value: T | null | undefined;
	onChange: (value: T | null) => void;
	placeholder?: string;
	required?: boolean;
	error?: string;
	disabled?: boolean;
	excludeValues?: T[];
}

function EnumPicker<T extends string>({
	label,
	options,
	value,
	onChange,
	placeholder = "Wybierz...",
	required = false,
	error,
	disabled = false,
	excludeValues = [],
}: EnumPickerProps<T>): React.ReactElement {
	const styles = useStyles();

	const filteredOptions = excludeValues.length
		? options.filter((o) => !excludeValues.includes(o.value))
		: options;

	const selectedOption = options.find((o) => o.value === value);

	return (
		<Field
			label={required ? `${label} *` : label}
			validationState={error ? "error" : "none"}
			validationMessage={error}
		>
			<Dropdown
				className={styles.dropdown}
				placeholder={placeholder}
				value={selectedOption?.label ?? ""}
				selectedOptions={value ? [value] : []}
				onOptionSelect={(_, data) => {
					const newValue = data.optionValue as T | undefined;
					onChange(newValue ?? null);
				}}
				disabled={disabled}
			>
				{filteredOptions.map((option) => (
					<Option key={option.value} value={option.value}>
						{option.label}
					</Option>
				))}
			</Dropdown>
		</Field>
	);
}

export default EnumPicker;
