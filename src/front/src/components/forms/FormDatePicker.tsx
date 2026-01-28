import {
	DatePicker,
	type DatePickerProps,
} from "@fluentui/react-datepicker-compat";
import type React from "react";
import {
	type Control,
	Controller,
	type FieldPath,
	type FieldValues,
} from "react-hook-form";
import FormField, { type FormFieldProps } from "./FormField";

export interface FormDatePickerProps<TFieldValues extends FieldValues>
	extends Omit<FormFieldProps, "children" | "error"> {
	name: FieldPath<TFieldValues>;
	control: Control<TFieldValues>;
	datePickerProps?: Omit<DatePickerProps, "value" | "onSelectDate">;
}

const FormDatePicker = <TFieldValues extends FieldValues>({
	name,
	control,
	datePickerProps,
	...fieldProps
}: FormDatePickerProps<TFieldValues>): React.ReactElement => {
	return (
		<Controller
			name={name}
			control={control}
			render={({ field, fieldState }) => (
				<FormField {...fieldProps} error={fieldState.error?.message}>
					<DatePicker
						{...datePickerProps}
						value={field.value ? new Date(field.value) : null}
						onSelectDate={(date) => {
							field.onChange(date?.toISOString() ?? null);
						}}
					/>
				</FormField>
			)}
		/>
	);
};

export default FormDatePicker;
