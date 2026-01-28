import type { InputProps } from "@fluentui/react-components";
import { Input } from "@fluentui/react-components";
import type React from "react";
import {
	type Control,
	Controller,
	type FieldPath,
	type FieldValues,
} from "react-hook-form";
import FormField, { type FormFieldProps } from "./FormField";

export interface FormInputProps<TFieldValues extends FieldValues>
	extends Omit<FormFieldProps, "children" | "error"> {
	name: FieldPath<TFieldValues>;
	control: Control<TFieldValues>;
	inputProps?: Omit<InputProps, "value" | "onChange">;
}

const FormInput = <TFieldValues extends FieldValues>({
	name,
	control,
	inputProps,
	...fieldProps
}: FormInputProps<TFieldValues>): React.ReactElement => {
	return (
		<Controller
			name={name}
			control={control}
			render={({ field, fieldState }) => (
				<FormField {...fieldProps} error={fieldState.error?.message}>
					<Input
						{...inputProps}
						{...field}
						value={field.value ?? ""}
						onChange={(_, data) => field.onChange(data.value)}
					/>
				</FormField>
			)}
		/>
	);
};

export default FormInput;
