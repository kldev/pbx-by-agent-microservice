import { Button, makeStyles, Text, tokens } from "@fluentui/react-components";
import { NavigationRegular } from "@fluentui/react-icons";
import type React from "react";
import UserMenu from "./UserMenu";

const useStyles = makeStyles({
	header: {
		display: "flex",
		alignItems: "center",
		justifyContent: "space-between",
		height: "48px",
		padding: `0 ${tokens.spacingHorizontalL}`,
		backgroundColor: tokens.colorNeutralBackground1,
		borderBottom: `1px solid ${tokens.colorNeutralStroke1}`,
		position: "sticky",
		top: 0,
		zIndex: 100,
	},
	left: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalM,
	},
	logo: {
		fontWeight: tokens.fontWeightSemibold,
		fontSize: tokens.fontSizeBase400,
		color: tokens.colorBrandForeground1,
	},
	right: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalM,
	},
	menuButton: {
		"@media (min-width: 768px)": {
			display: "none",
		},
	},
});

interface HeaderProps {
	onMenuClick: () => void;
}

const Header: React.FC<HeaderProps> = ({ onMenuClick }) => {
	const styles = useStyles();

	return (
		<header className={styles.header}>
			<div className={styles.left}>
				<Button
					appearance="subtle"
					icon={<NavigationRegular />}
					onClick={onMenuClick}
					className={styles.menuButton}
				/>
				<Text className={styles.logo}>
					{import.meta.env.VITE_APP_NAME || "ERP by Agent"}
				</Text>
			</div>
			<div className={styles.right}>
				<UserMenu />
			</div>
		</header>
	);
};

export default Header;
