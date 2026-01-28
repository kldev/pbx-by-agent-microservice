import { Badge, makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";

export interface SystemUserRolesBadgeProps {
	roles?: string[] | null;
}

const roleLabels: Record<string, string> = {
	Root: "Super Admin",
	Admin: "Administrator",
	ResponsiblePerson: "Użytkownik",
	SalesPerson: "Sprzedawca",
	PreSales: "Pre-Sales",
	HrInternational: "HR International",
	HrPoland: "HR Polska",
	OpsInternational: "Operacje International",
	OpsPoland: "Operacje Polska",
	Finance: "Finanse",
	Administration: "Administracja",
	Sales: "Sales",
	Recruiter: "Rekruter",
};

const useStyles = makeStyles({
	container: {
		display: "flex",
		gap: tokens.spacingHorizontalXS,
		flexWrap: "wrap",
	},
});

const SystemUserRolesBadge: React.FC<SystemUserRolesBadgeProps> = ({
	roles,
}) => {
	const styles = useStyles();

	if (!roles || roles.length === 0) {
		return (
			<Badge appearance="outline" color="subtle">
				Brak ról
			</Badge>
		);
	}

	return (
		<div className={styles.container}>
			{roles.map((role) => (
				<Badge key={role} appearance="tint" color="brand" size="small">
					{roleLabels[role] ?? role}
				</Badge>
			))}
		</div>
	);
};

export default SystemUserRolesBadge;
