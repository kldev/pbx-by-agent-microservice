import {
	Breadcrumb,
	BreadcrumbDivider,
	BreadcrumbItem,
	makeStyles,
	Text,
	tokens,
} from "@fluentui/react-components";
import type React from "react";
import { Link } from "react-router";

const useStyles = makeStyles({
	header: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalS,
		padding: tokens.spacingVerticalL,
		paddingBottom: tokens.spacingVerticalM,
	},
	titleRow: {
		display: "flex",
		alignItems: "center",
		justifyContent: "space-between",
		flexWrap: "wrap",
		gap: tokens.spacingHorizontalM,
	},
	title: {
		fontSize: tokens.fontSizeBase600,
		fontWeight: tokens.fontWeightSemibold,
		lineHeight: tokens.lineHeightBase600,
	},
	actions: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
	},
	breadcrumbLink: {
		textDecoration: "none",
		color: tokens.colorBrandForeground1,
		":hover": {
			textDecoration: "underline",
		},
	},
	breadcrumbCurrent: {
		color: tokens.colorNeutralForeground1,
	},
});

export interface BreadcrumbItemType {
	label: string;
	href?: string;
}

export interface PageHeaderProps {
	title: string;
	breadcrumbs?: BreadcrumbItemType[];
	actions?: React.ReactNode;
}

const PageHeader: React.FC<PageHeaderProps> = ({
	title,
	breadcrumbs,
	actions,
}) => {
	const styles = useStyles();

	return (
		<div className={styles.header}>
			{breadcrumbs && breadcrumbs.length > 0 && (
				<Breadcrumb>
					{breadcrumbs.map((item, index) => (
						<BreadcrumbItem key={item.label}>
							{item.href ? (
								<Link to={item.href} className={styles.breadcrumbLink}>
									{item.label}
								</Link>
							) : (
								<span className={styles.breadcrumbCurrent}>{item.label}</span>
							)}
							{index < breadcrumbs.length - 1 && <BreadcrumbDivider />}
						</BreadcrumbItem>
					))}
				</Breadcrumb>
			)}
			<div className={styles.titleRow}>
				<Text className={styles.title}>{title}</Text>
				{actions && <div className={styles.actions}>{actions}</div>}
			</div>
		</div>
	);
};

export default PageHeader;
