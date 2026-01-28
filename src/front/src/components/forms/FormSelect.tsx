import {
	Dropdown,
	type DropdownProps,
	Option,
} from "@fluentui/react-components";
import type React from "react";
import {
	type Control,
	Controller,
	type FieldPath,
	type FieldValues,
} from "react-hook-form";
import FormField, { type FormFieldProps } from "./FormField";

export interface SelectOption {
	value: string;
	label: string;
}

export interface FormSelectProps<TFieldValues extends FieldValues>
	extends Omit<FormFieldProps, "children" | "error"> {
	name: FieldPath<TFieldValues>;
	control: Control<TFieldValues>;
	options: SelectOption[];
	placeholder?: string;
	dropdownProps?: Omit<DropdownProps, "value" | "onOptionSelect">;
}

const FormSelect = <TFieldValues extends FieldValues>({
	name,
	control,
	options,
	placeholder,
	dropdownProps,
	...fieldProps
}: FormSelectProps<TFieldValues>): React.ReactElement => {
	return (
		<Controller
			name={name}
			control={control}
			render={({ field, fieldState }) => {
				const selectedOption = options.find((opt) => opt.value === field.value);
				return (
					<FormField {...fieldProps} error={fieldState.error?.message}>
						<Dropdown
							{...dropdownProps}
							placeholder={placeholder}
							value={selectedOption?.label ?? ""}
							selectedOptions={field.value ? [field.value] : []}
							onOptionSelect={(_, data) => {
								field.onChange(data.optionValue);
							}}
						>
							{options.map((option) => (
								<Option key={option.value} value={option.value}>
									{option.label}
								</Option>
							))}
						</Dropdown>
					</FormField>
				);
			}}
		/>
	);
};

export default FormSelect;
