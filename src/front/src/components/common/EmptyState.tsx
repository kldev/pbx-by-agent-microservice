import { Button, makeStyles, Text, tokens } from "@fluentui/react-components";
import { DocumentRegular } from "@fluentui/react-icons";
import type React from "react";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		alignItems: "center",
		justifyContent: "center",
		padding: tokens.spacingVerticalXXXL,
		textAlign: "center",
		gap: tokens.spacingVerticalM,
	},
	icon: {
		fontSize: "48px",
		color: tokens.colorNeutralForeground3,
	},
	title: {
		fontSize: tokens.fontSizeBase500,
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorNeutralForeground1,
	},
	description: {
		fontSize: tokens.fontSizeBase300,
		color: tokens.colorNeutralForeground3,
		maxWidth: "320px",
	},
});

export interface EmptyStateProps {
	icon?: React.ReactNode;
	title: string;
	description?: string;
	actionLabel?: string;
	onAction?: () => void;
}

const EmptyState: React.FC<EmptyStateProps> = ({
	icon,
	title,
	description,
	actionLabel,
	onAction,
}) => {
	const styles = useStyles();

	return (
		<div className={styles.container}>
			<div className={styles.icon}>{icon || <DocumentRegular />}</div>
			<Text className={styles.title}>{title}</Text>
			{description && <Text className={styles.description}>{description}</Text>}
			{actionLabel && onAction && (
				<Button appearance="primary" onClick={onAction}>
					{actionLabel}
				</Button>
			)}
		</div>
	);
};

export default EmptyState;
