import { makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";
import { useState } from "react";
import { Outlet } from "react-router";
import Header from "./Header";
import Sidebar from "./Sidebar";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		height: "100vh",
		overflow: "hidden",
	},
	body: {
		display: "flex",
		flex: 1,
		overflow: "hidden",
	},
	main: {
		flex: 1,
		overflow: "auto",
		backgroundColor: tokens.colorNeutralBackground3,
		padding: tokens.spacingHorizontalL,
	},
	overlay: {
		position: "fixed",
		inset: 0,
		backgroundColor: "rgba(0, 0, 0, 0.4)",
		zIndex: 50,
		"@media (min-width: 768px)": {
			display: "none",
		},
	},
	mobileSidebar: {
		position: "fixed",
		left: 0,
		top: "48px",
		bottom: 0,
		zIndex: 60,
		transform: "translateX(-100%)",
		transition: "transform 0.2s ease",
		"@media (min-width: 768px)": {
			display: "none",
		},
	},
	mobileSidebarOpen: {
		transform: "translateX(0)",
	},
	desktopSidebar: {
		display: "none",
		"@media (min-width: 768px)": {
			display: "block",
		},
	},
});

const AppShell: React.FC = () => {
	const styles = useStyles();
	const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

	const toggleMobileMenu = () => {
		setMobileMenuOpen((prev) => !prev);
	};

	const closeMobileMenu = () => {
		setMobileMenuOpen(false);
	};

	return (
		<div className={styles.container}>
			<Header onMenuClick={toggleMobileMenu} />
			<div className={styles.body}>
				{/* Mobile overlay */}
				{mobileMenuOpen && (
					<div className={styles.overlay} onClick={closeMobileMenu} />
				)}

				{/* Mobile sidebar */}
				<div
					className={`${styles.mobileSidebar} ${mobileMenuOpen ? styles.mobileSidebarOpen : ""}`}
				>
					<Sidebar />
				</div>

				{/* Desktop sidebar */}
				<div className={styles.desktopSidebar}>
					<Sidebar />
				</div>

				<main className={styles.main}>
					<Outlet />
				</main>
			</div>
		</div>
	);
};

export default AppShell;
