import {
	Avatar,
	Menu,
	MenuItem,
	MenuList,
	MenuPopover,
	MenuTrigger,
	makeStyles,
	tokens,
} from "@fluentui/react-components";
import {
	PersonRegular,
	SettingsRegular,
	SignOutRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useNavigate } from "react-router";
import { ROUTES } from "../../routes";
import { useAuthStore } from "../../stores";

const useStyles = makeStyles({
	trigger: {
		cursor: "pointer",
	},
	menuItem: {
		minWidth: "180px",
	},
	userInfo: {
		padding: `${tokens.spacingVerticalS} ${tokens.spacingHorizontalM}`,
		borderBottom: `1px solid ${tokens.colorNeutralStroke1}`,
		marginBottom: tokens.spacingVerticalXS,
	},
	userName: {
		fontWeight: tokens.fontWeightSemibold,
		display: "block",
	},
	userEmail: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
});

const UserMenu: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { user, clearUser } = useAuthStore();

	const handleLogout = () => {
		clearUser();
		navigate(ROUTES.LOGIN);
	};

	const initials = user
		? `${user.firstName?.charAt(0) || ""}${user.lastName?.charAt(0) || ""}`
		: "?";

	const fullName = user
		? `${user.firstName || ""} ${user.lastName || ""}`.trim()
		: "Użytkownik";

	return (
		<Menu>
			<MenuTrigger disableButtonEnhancement>
				<Avatar
					name={fullName}
					initials={initials}
					size={32}
					className={styles.trigger}
					color="brand"
				/>
			</MenuTrigger>
			<MenuPopover>
				<MenuList>
					<div className={styles.userInfo}>
						<span className={styles.userName}>{fullName}</span>
						<span className={styles.userEmail}>{user?.email}</span>
					</div>
					<MenuItem
						icon={<PersonRegular />}
						className={styles.menuItem}
						disabled
					>
						Mój profil
					</MenuItem>
					<MenuItem
						icon={<SettingsRegular />}
						className={styles.menuItem}
						disabled
					>
						Ustawienia
					</MenuItem>
					<MenuItem
						icon={<SignOutRegular />}
						className={styles.menuItem}
						onClick={handleLogout}
					>
						Wyloguj się
					</MenuItem>
				</MenuList>
			</MenuPopover>
		</Menu>
	);
};

export default UserMenu;
