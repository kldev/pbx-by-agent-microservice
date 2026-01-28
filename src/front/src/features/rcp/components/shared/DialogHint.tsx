import { makeStyles, Text, tokens } from "@fluentui/react-components";
import { InfoRegular } from "@fluentui/react-icons";
import type React from "react";

const useStyles = makeStyles({
	container: {
		display: "flex",
		gap: tokens.spacingHorizontalS,
		padding: tokens.spacingVerticalS,
		backgroundColor: tokens.colorNeutralBackground3,
		borderRadius: tokens.borderRadiusMedium,
		marginBottom: tokens.spacingVerticalM,
	},
	icon: {
		color: tokens.colorBrandForeground1,
		fontSize: "16px",
		flexShrink: 0,
		marginTop: "2px",
	},
	content: {
		color: tokens.colorNeutralForeground2,
		fontSize: tokens.fontSizeBase200,
		lineHeight: "1.4",
	},
});

interface DialogHintProps {
	children: React.ReactNode;
}

const DialogHint: React.FC<DialogHintProps> = ({ children }) => {
	const styles = useStyles();

	return (
		<div className={styles.container}>
			<InfoRegular className={styles.icon} />
			<Text className={styles.content}>{children}</Text>
		</div>
	);
};

export default DialogHint;
