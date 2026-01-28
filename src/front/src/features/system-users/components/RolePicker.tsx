import { Checkbox, makeStyles, Text, tokens } from "@fluentui/react-components";
import { ShieldRegular, SettingsRegular } from "@fluentui/react-icons";
import type React from "react";
import type { AppRole } from "../../../api/identity/models";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXS,
	},
	roleItem: {
		display: "flex",
		alignItems: "flex-start",
		gap: tokens.spacingHorizontalS,
		padding: tokens.spacingVerticalS,
		borderRadius: tokens.borderRadiusMedium,
		backgroundColor: tokens.colorNeutralBackground2,
		cursor: "pointer",
		":hover": {
			backgroundColor: tokens.colorNeutralBackground2Hover,
		},
	},
	roleItemSelected: {
		backgroundColor: tokens.colorBrandBackground2,
		":hover": {
			backgroundColor: tokens.colorBrandBackground2Hover,
		},
	},
	roleIcon: {
		marginTop: "2px",
		color: tokens.colorNeutralForeground2,
	},
	roleContent: {
		flex: 1,
		display: "flex",
		flexDirection: "column",
	},
	roleName: {
		fontWeight: tokens.fontWeightSemibold,
	},
	roleDescription: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground2,
	},
	categoryTitle: {
		fontSize: tokens.fontSizeBase200,
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorNeutralForeground2,
		textTransform: "uppercase",
		marginTop: tokens.spacingVerticalM,
		marginBottom: tokens.spacingVerticalXS,
	},
});

interface RoleDefinition {
	value: AppRole;
	label: string;
	description: string;
	category: "admin" | "sales" | "ops" | "other";
	icon: React.ReactNode;
}

const roleDefinitions: RoleDefinition[] = [
	// Admin roles
	{
		value: "Root",
		label: "Super Admin",
		description: "Pełny dostęp do systemu, wszystkie uprawnienia",
		category: "admin",
		icon: <ShieldRegular />,
	},
	{
		value: "Admin",
		label: "Administrator",
		description: "Zarządzanie użytkownikami i konfiguracją",
		category: "admin",
		icon: <SettingsRegular />,
	},

	// Ops roles
	{
		value: "Ops",
		label: "Operacje (OPS)",
		description: "Obsługa operacyjna",
		category: "ops",
		icon: <SettingsRegular />,
	},
	// Support
	{
		value: "Support",
		label: "Support",
		description: "Support",
		category: "ops",
		icon: <SettingsRegular />,
	},
	// User
	{
		value: "User",
		label: "User",
		description: "End-user",
		category: "ops",
		icon: <SettingsRegular />,
	},
];

const categoryLabels: Record<RoleDefinition["category"], string> = {
	admin: "Administracja",
	ops: "Operacje",
	other: "Inne",
	sales: "Sprzedaż",
};

const categoryOrder: RoleDefinition["category"][] = [
	"admin",
	"sales",
	"ops",
	"other",
];

export interface RolePickerProps {
	selectedRoles: string[];
	onChange: (roles: string[]) => void;
	disabled?: boolean;
}

const RolePicker: React.FC<RolePickerProps> = ({
	selectedRoles,
	onChange,
	disabled = false,
}) => {
	const styles = useStyles();

	const handleToggle = (roleValue: string) => {
		if (disabled) return;

		if (selectedRoles.includes(roleValue)) {
			onChange(selectedRoles.filter((r) => r !== roleValue));
		} else {
			onChange([...selectedRoles, roleValue]);
		}
	};

	const rolesByCategory = categoryOrder.map((category) => ({
		category,
		label: categoryLabels[category],
		roles: roleDefinitions.filter((r) => r.category === category),
	}));

	return (
		<div className={styles.container}>
			{rolesByCategory.map(({ category, label, roles }) => (
				<div key={category}>
					<Text className={styles.categoryTitle}>{label}</Text>
					{roles.map((role) => {
						const isSelected = selectedRoles.includes(role.value);
						return (
							<div
								key={role.value}
								className={`${styles.roleItem} ${isSelected ? styles.roleItemSelected : ""}`}
								onClick={() => handleToggle(role.value)}
							>
								<Checkbox
									checked={isSelected}
									onChange={() => handleToggle(role.value)}
									disabled={disabled}
								/>
								<span className={styles.roleIcon}>{role.icon}</span>
								<div className={styles.roleContent}>
									<Text className={styles.roleName}>{role.label}</Text>
									<Text className={styles.roleDescription}>
										{role.description}
									</Text>
								</div>
							</div>
						);
					})}
				</div>
			))}
		</div>
	);
};

export default RolePicker;
