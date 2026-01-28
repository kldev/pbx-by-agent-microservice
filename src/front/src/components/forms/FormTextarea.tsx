import { Textarea, type TextareaProps } from "@fluentui/react-components";
import type React from "react";
import {
	type Control,
	Controller,
	type FieldPath,
	type FieldValues,
} from "react-hook-form";
import FormField, { type FormFieldProps } from "./FormField";

export interface FormTextareaProps<TFieldValues extends FieldValues>
	extends Omit<FormFieldProps, "children" | "error"> {
	name: FieldPath<TFieldValues>;
	control: Control<TFieldValues>;
	textareaProps?: Omit<TextareaProps, "value" | "onChange">;
}

const FormTextarea = <TFieldValues extends FieldValues>({
	name,
	control,
	textareaProps,
	...fieldProps
}: FormTextareaProps<TFieldValues>): React.ReactElement => {
	return (
		<Controller
			name={name}
			control={control}
			render={({ field, fieldState }) => (
				<FormField {...fieldProps} error={fieldState.error?.message}>
					<Textarea
						{...textareaProps}
						{...field}
						value={field.value ?? ""}
						onChange={(_, data) => field.onChange(data.value)}
					/>
				</FormField>
			)}
		/>
	);
};

export default FormTextarea;
