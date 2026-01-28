import { Button, makeStyles, Text, tokens } from "@fluentui/react-components";
import {
	CallFilled,
	CallForwardFilled,
	CallForwardRegular,
	CallRegular,
	ChevronLeftRegular,
	ChevronRightRegular,
	ClockFilled,
	ClockRegular,
	PeopleFilled,
	PeopleRegular,
	PersonAccountsFilled,
	PersonAccountsRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { NavLink } from "react-router";
import { AppRole } from "../../api/identity/models";
import { ROUTES } from "../../routes";
import { useAuthStore, useUIStore } from "../../stores";

const useStyles = makeStyles({
	sidebar: {
		display: "flex",
		flexDirection: "column",
		height: "100%",
		backgroundColor: tokens.colorNeutralBackground2,
		borderRight: `1px solid ${tokens.colorNeutralStroke1}`,
		transition: "width 0.2s ease",
		overflowY: "auto",
	},
	sidebarExpanded: {
		width: "240px",
	},
	sidebarCollapsed: {
		width: "56px",
	},
	nav: {
		flex: 1,
		padding: tokens.spacingVerticalM,
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXS,
	},
	navItem: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalM,
		padding: `${tokens.spacingVerticalS} ${tokens.spacingHorizontalM}`,
		borderRadius: tokens.borderRadiusMedium,
		textDecoration: "none",
		color: tokens.colorNeutralForeground1,
		transition: "background-color 0.1s ease",
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1Hover,
		},
	},
	navItemActive: {
		backgroundColor: tokens.colorBrandBackground2,
		color: tokens.colorBrandForeground1,
		":hover": {
			backgroundColor: tokens.colorBrandBackground2Hover,
		},
	},
	navItemCollapsed: {
		justifyContent: "center",
		padding: tokens.spacingVerticalS,
	},
	navText: {
		whiteSpace: "nowrap",
		overflow: "hidden",
	},
	navGroup: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXS,
	},
	navGroupLabel: {
		fontSize: tokens.fontSizeBase200,
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorNeutralForeground3,
		textTransform: "uppercase",
		letterSpacing: "0.5px",
		padding: `${tokens.spacingVerticalM} ${tokens.spacingHorizontalM} ${tokens.spacingVerticalXS}`,
		whiteSpace: "nowrap",
		overflow: "hidden",
	},
	navGroupLabelCollapsed: {
		textAlign: "center",
		padding: `${tokens.spacingVerticalM} 0 ${tokens.spacingVerticalXS}`,
	},
	navDivider: {
		height: "1px",
		backgroundColor: tokens.colorNeutralStroke2,
		margin: `${tokens.spacingVerticalS} 0`,
	},
	footer: {
		padding: tokens.spacingVerticalM,
		borderTop: `1px solid ${tokens.colorNeutralStroke1}`,
	},
	collapseButton: {
		width: "100%",
	},
});

interface NavItemProps {
	to: string;
	icon: React.ReactNode;
	activeIcon: React.ReactNode;
	label: string;
	collapsed: boolean;
	end?: boolean;
}

const NavItem: React.FC<NavItemProps> = ({
	to,
	icon,
	activeIcon,
	label,
	collapsed,
	end,
}) => {
	const styles = useStyles();

	return (
		<NavLink
			to={to}
			end={end}
			className={({ isActive }) =>
				`${styles.navItem} ${isActive ? styles.navItemActive : ""} ${collapsed ? styles.navItemCollapsed : ""}`
			}
			title={collapsed ? label : undefined}
		>
			{({ isActive }) => (
				<>
					{isActive ? activeIcon : icon}
					{!collapsed && <Text className={styles.navText}>{label}</Text>}
				</>
			)}
		</NavLink>
	);
};

interface NavGroupLabelProps {
	label: string;
	shortLabel: string;
	collapsed: boolean;
}

const NavGroupLabel: React.FC<NavGroupLabelProps> = ({
	label,
	shortLabel,
	collapsed,
}) => {
	const styles = useStyles();
	return (
		<Text
			className={`${styles.navGroupLabel} ${collapsed ? styles.navGroupLabelCollapsed : ""}`}
			title={collapsed ? label : undefined}
		>
			{collapsed ? shortLabel : label}
		</Text>
	);
};

const ADMIN_ROLES: AppRole[] = [AppRole.Admin, AppRole.Root];

const Sidebar: React.FC = () => {
	const styles = useStyles();
	const { isSidebarCollapsed, toggleSidebar } = useUIStore();
	const user = useAuthStore((state) => state.user);

	const isAdmin = user?.roles?.some((role) => ADMIN_ROLES.includes(role));

	return (
		<aside
			className={`${styles.sidebar} ${isSidebarCollapsed ? styles.sidebarCollapsed : styles.sidebarExpanded}`}
		>
			<nav className={styles.nav}>
				{/* Grupa: PBX */}
				<div className={styles.navDivider} />
				<NavGroupLabel
					label="PBX"
					shortLabel="P"
					collapsed={isSidebarCollapsed}
				/>
				<div className={styles.navGroup}>
					<NavItem
						to={ROUTES.CDR_LIST}
						icon={<CallRegular />}
						activeIcon={<CallFilled />}
						label="CDR"
						collapsed={isSidebarCollapsed}
					/>
					<NavItem
						to={ROUTES.ANSWERING_RULES_LIST}
						icon={<CallForwardRegular />}
						activeIcon={<CallForwardFilled />}
						label="Reguły odpowiadania"
						collapsed={isSidebarCollapsed}
					/>
				</div>

				{/* Grupa: RCP */}
				<div className={styles.navDivider} />
				<NavGroupLabel
					label="Czas Pracy"
					shortLabel="R"
					collapsed={isSidebarCollapsed}
				/>
				<div className={styles.navGroup}>
					<NavItem
						to={ROUTES.RCP}
						icon={<ClockRegular />}
						activeIcon={<ClockFilled />}
						label="RCP"
						collapsed={isSidebarCollapsed}
					/>
				</div>

				{/* Grupa: Systemowe - tylko dla adminów */}
				{isAdmin && (
					<>
						<div className={styles.navDivider} />
						<NavGroupLabel
							label="Systemowe"
							shortLabel="S"
							collapsed={isSidebarCollapsed}
						/>
						<div className={styles.navGroup}>
							<NavItem
								to={ROUTES.TEAMS_LIST}
								icon={<PeopleRegular />}
								activeIcon={<PeopleFilled />}
								label="Zespoły"
								collapsed={isSidebarCollapsed}
							/>
							<NavItem
								to={ROUTES.SYSTEM_USERS_LIST}
								icon={<PersonAccountsRegular />}
								activeIcon={<PersonAccountsFilled />}
								label="Użytkownicy"
								collapsed={isSidebarCollapsed}
							/>
						</div>
					</>
				)}
			</nav>
			<div className={styles.footer}>
				<Button
					appearance="subtle"
					icon={
						isSidebarCollapsed ? (
							<ChevronRightRegular />
						) : (
							<ChevronLeftRegular />
						)
					}
					onClick={toggleSidebar}
					className={styles.collapseButton}
				>
					{!isSidebarCollapsed && "Zwiń"}
				</Button>
			</div>
		</aside>
	);
};

export default Sidebar;
