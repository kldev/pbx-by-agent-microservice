import { Checkbox, type CheckboxProps } from "@fluentui/react-components";
import type React from "react";
import {
	type Control,
	Controller,
	type FieldPath,
	type FieldValues,
} from "react-hook-form";
import FormField, { type FormFieldProps } from "./FormField";

export interface FormCheckboxProps<TFieldValues extends FieldValues>
	extends Omit<FormFieldProps, "children" | "error" | "label"> {
	name: FieldPath<TFieldValues>;
	control: Control<TFieldValues>;
	checkboxLabel: string;
	checkboxProps?: Omit<CheckboxProps, "checked" | "onChange">;
}

const FormCheckbox = <TFieldValues extends FieldValues>({
	name,
	control,
	checkboxLabel,
	checkboxProps,
	...fieldProps
}: FormCheckboxProps<TFieldValues>): React.ReactElement => {
	return (
		<Controller
			name={name}
			control={control}
			render={({ field, fieldState }) => (
				<FormField {...fieldProps} error={fieldState.error?.message}>
					<Checkbox
						{...checkboxProps}
						label={checkboxLabel}
						checked={!!field.value}
						onChange={(_, data) => field.onChange(data.checked)}
					/>
				</FormField>
			)}
		/>
	);
};

export default FormCheckbox;
