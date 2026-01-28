import type { FieldProps } from "@fluentui/react-components";
import { Field, makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";

const useStyles = makeStyles({
	field: {
		marginBottom: tokens.spacingVerticalM,
	},
});

export interface FormFieldProps extends Omit<FieldProps, "validationState"> {
	error?: string;
	children: React.ReactNode;
}

const FormField: React.FC<FormFieldProps> = ({
	error,
	children,
	className,
	...props
}) => {
	const styles = useStyles();

	return (
		<Field
			{...props}
			className={`${styles.field} ${className || ""}`}
			validationState={error ? "error" : undefined}
			validationMessage={error}
		>
			{children}
		</Field>
	);
};

export default FormField;
