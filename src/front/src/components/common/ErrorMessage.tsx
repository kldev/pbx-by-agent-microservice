import {
	Button,
	MessageBar,
	MessageBarBody,
	MessageBarTitle,
	makeStyles,
	tokens,
} from "@fluentui/react-components";
import type React from "react";

const useStyles = makeStyles({
	container: {
		padding: tokens.spacingVerticalM,
	},
	actions: {
		marginTop: tokens.spacingVerticalS,
	},
});

export interface ErrorMessageProps {
	title?: string;
	message: string;
	onRetry?: () => void;
}

const ErrorMessage: React.FC<ErrorMessageProps> = ({
	title = "Wystąpił błąd",
	message,
	onRetry,
}) => {
	const styles = useStyles();

	return (
		<div className={styles.container}>
			<MessageBar intent="error">
				<MessageBarBody>
					<MessageBarTitle>{title}</MessageBarTitle>
					{message}
				</MessageBarBody>
			</MessageBar>
			{onRetry && (
				<div className={styles.actions}>
					<Button appearance="secondary" onClick={onRetry}>
						Spróbuj ponownie
					</Button>
				</div>
			)}
		</div>
	);
};

export default ErrorMessage;
