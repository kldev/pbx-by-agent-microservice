import {
	Button,
	makeStyles,
	mergeClasses,
	Text,
	tokens,
} from "@fluentui/react-components";
import {
	ChevronDownRegular,
	ChevronUpRegular,
	InfoRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useEffect, useState } from "react";

const useStyles = makeStyles({
	container: {
		backgroundColor: tokens.colorNeutralBackground1,
		borderRadius: tokens.borderRadiusMedium,
		marginBottom: tokens.spacingVerticalM,
		border: `1px solid ${tokens.colorNeutralStroke2}`,
		boxShadow: tokens.shadow4,
	},
	header: {
		display: "flex",
		alignItems: "center",
		justifyContent: "space-between",
		padding: `${tokens.spacingVerticalS} ${tokens.spacingHorizontalM}`,
		cursor: "pointer",
		backgroundColor: tokens.colorBrandBackground2,
		borderRadius: `${tokens.borderRadiusMedium} ${tokens.borderRadiusMedium} 0 0`,
		borderBottom: `1px solid ${tokens.colorNeutralStroke2}`,
		":hover": {
			backgroundColor: tokens.colorBrandBackground2Hover,
		},
	},
	headerLeft: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
	},
	icon: {
		color: tokens.colorBrandForeground1,
		fontSize: "20px",
	},
	title: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorBrandForeground1,
	},
	content: {
		padding: tokens.spacingHorizontalM,
		color: tokens.colorNeutralForeground1,
		fontSize: tokens.fontSizeBase300,
		lineHeight: "1.6",
	},
	contentHidden: {
		display: "none",
	},
	section: {
		marginBottom: tokens.spacingVerticalM,
		padding: tokens.spacingHorizontalM,
		backgroundColor: tokens.colorNeutralBackground2,
		borderRadius: tokens.borderRadiusMedium,
		borderLeft: `3px solid ${tokens.colorBrandStroke1}`,
	},
	sectionTitle: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorNeutralForeground1,
		marginBottom: tokens.spacingVerticalS,
		display: "block",
		fontSize: tokens.fontSizeBase400,
	},
	list: {
		margin: `${tokens.spacingVerticalS} 0 0 0`,
		paddingLeft: tokens.spacingHorizontalXL,
		listStyleType: "disc",
	},
	listItem: {
		marginBottom: tokens.spacingVerticalS,
		paddingLeft: tokens.spacingHorizontalXS,
	},
	highlight: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorBrandForeground1,
		backgroundColor: tokens.colorBrandBackground2,
		padding: `${tokens.spacingVerticalXXS} ${tokens.spacingHorizontalXS}`,
		borderRadius: tokens.borderRadiusSmall,
	},
	warning: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorPaletteRedForeground1,
		backgroundColor: tokens.colorPaletteRedBackground1,
		padding: `${tokens.spacingVerticalXXS} ${tokens.spacingHorizontalXS}`,
		borderRadius: tokens.borderRadiusSmall,
	},
});

interface SectionHintProps {
	title: string;
	storageKey?: string;
	defaultExpanded?: boolean;
	children: React.ReactNode;
}

const SectionHint: React.FC<SectionHintProps> = ({
	title,
	storageKey,
	defaultExpanded = true,
	children,
}) => {
	const styles = useStyles();
	const [isExpanded, setIsExpanded] = useState(defaultExpanded);

	// Załaduj preferencje z localStorage
	useEffect(() => {
		if (storageKey) {
			const saved = localStorage.getItem(`hint_${storageKey}`);
			if (saved !== null) {
				setIsExpanded(saved === "true");
			}
		}
	}, [storageKey]);

	const handleToggle = () => {
		const newValue = !isExpanded;
		setIsExpanded(newValue);
		if (storageKey) {
			localStorage.setItem(`hint_${storageKey}`, String(newValue));
		}
	};

	return (
		<div className={styles.container}>
			<div
				className={styles.header}
				onClick={handleToggle}
				onKeyDown={(e) => e.key === "Enter" && handleToggle()}
				role="button"
				tabIndex={0}
			>
				<div className={styles.headerLeft}>
					<InfoRegular className={styles.icon} />
					<Text className={styles.title}>{title}</Text>
				</div>
				<Button
					appearance="subtle"
					size="small"
					icon={isExpanded ? <ChevronUpRegular /> : <ChevronDownRegular />}
				/>
			</div>
			<div
				className={mergeClasses(
					styles.content,
					!isExpanded && styles.contentHidden,
				)}
			>
				{children}
			</div>
		</div>
	);
};

export default SectionHint;

// Pomocnicze komponenty do formatowania treści legendy
export const HintSection: React.FC<{
	title?: string;
	children: React.ReactNode;
}> = ({ title, children }) => {
	const styles = useStyles();
	return (
		<div className={styles.section}>
			{title && <Text className={styles.sectionTitle}>{title}</Text>}
			{children}
		</div>
	);
};

export const HintList: React.FC<{ children: React.ReactNode }> = ({
	children,
}) => {
	const styles = useStyles();
	return <ul className={styles.list}>{children}</ul>;
};

export const HintListItem: React.FC<{ children: React.ReactNode }> = ({
	children,
}) => {
	const styles = useStyles();
	return <li className={styles.listItem}>{children}</li>;
};

export const HintHighlight: React.FC<{ children: React.ReactNode }> = ({
	children,
}) => {
	const styles = useStyles();
	return <span className={styles.highlight}>{children}</span>;
};

export const HintWarning: React.FC<{ children: React.ReactNode }> = ({
	children,
}) => {
	const styles = useStyles();
	return <span className={styles.warning}>{children}</span>;
};
