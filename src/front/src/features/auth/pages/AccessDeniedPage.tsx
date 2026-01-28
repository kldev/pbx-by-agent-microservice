import {
	Button,
	Card,
	makeStyles,
	Text,
	Title1,
	tokens,
} from "@fluentui/react-components";
import { ShieldErrorRegular } from "@fluentui/react-icons";
import type React from "react";
import { useNavigate } from "react-router";
import { ROUTES } from "../../../routes";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		alignItems: "center",
		justifyContent: "center",
		minHeight: "100vh",
		backgroundColor: tokens.colorNeutralBackground2,
		padding: "24px",
	},
	card: {
		width: "100%",
		maxWidth: "450px",
		padding: "32px",
		textAlign: "center",
	},
	icon: {
		fontSize: "64px",
		color: tokens.colorPaletteRedForeground1,
		marginBottom: "16px",
	},
	title: {
		color: tokens.colorPaletteRedForeground1,
		marginBottom: "16px",
	},
	message: {
		color: tokens.colorNeutralForeground2,
		marginBottom: "24px",
	},
	rolesSection: {
		backgroundColor: tokens.colorNeutralBackground3,
		borderRadius: tokens.borderRadiusMedium,
		padding: "16px",
		marginBottom: "24px",
	},
	rolesLabel: {
		fontWeight: tokens.fontWeightSemibold,
		marginBottom: "8px",
		display: "block",
	},
	rolesList: {
		display: "flex",
		flexWrap: "wrap",
		gap: "8px",
		justifyContent: "center",
	},
	roleBadge: {
		backgroundColor: tokens.colorBrandBackground,
		color: tokens.colorNeutralForegroundOnBrand,
		padding: "4px 12px",
		borderRadius: tokens.borderRadiusMedium,
		fontSize: tokens.fontSizeBase200,
		fontWeight: tokens.fontWeightSemibold,
	},
	actions: {
		display: "flex",
		gap: "12px",
		justifyContent: "center",
	},
});

interface AccessDeniedPageProps {
	requiredRoles?: string[];
}

const AccessDeniedPage: React.FC<AccessDeniedPageProps> = ({
	requiredRoles = [],
}) => {
	const styles = useStyles();
	const navigate = useNavigate();

	return (
		<div className={styles.container}>
			<Card className={styles.card}>
				<ShieldErrorRegular className={styles.icon} />
				<Title1 className={styles.title}>Brak dostępu</Title1>
				<Text className={styles.message} block>
					Nie masz uprawnień do wyświetlenia tej strony.
				</Text>

				{requiredRoles.length > 0 && (
					<div className={styles.rolesSection}>
						<Text className={styles.rolesLabel}>Wymagane role:</Text>
						<div className={styles.rolesList}>
							{requiredRoles.map((role) => (
								<span key={role} className={styles.roleBadge}>
									{role}
								</span>
							))}
						</div>
					</div>
				)}

				<div className={styles.actions}>
					<Button appearance="secondary" onClick={() => navigate(-1)}>
						Wróć
					</Button>
					<Button
						appearance="primary"
						onClick={() => navigate(ROUTES.DASHBOARD)}
					>
						Strona główna
					</Button>
				</div>
			</Card>
		</div>
	);
};

export default AccessDeniedPage;
