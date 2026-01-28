import { Badge, type BadgeProps } from "@fluentui/react-components";
import type React from "react";
import { Department } from "../../../api/identity/models";

export interface SystemUserDepartmentBadgeProps {
	department?: (typeof Department)[keyof typeof Department];
}

const departmentLabels: Record<
	(typeof Department)[keyof typeof Department],
	string
> = {
	[Department.Sales]: "Sprzedaż",
	[Department.Operations]: "Operacje",
	[Department.Finance]: "Finanse",
	[Department.Developers]: "It",
	[Department.Support]: "Support",
	[Department.Others]: "Pozostali",
};

const departmentColors: Record<
	(typeof Department)[keyof typeof Department],
	BadgeProps["color"]
> = {
	[Department.Sales]: "success",
	[Department.Operations]: "warning",
	[Department.Finance]: "subtle",
	[Department.Developers]: "important",
	[Department.Support]: "brand",
	[Department.Others]: "severe",
};

const SystemUserDepartmentBadge: React.FC<SystemUserDepartmentBadgeProps> = ({
	department,
}) => {
	if (!department) {
		return (
			<Badge appearance="outline" color="subtle">
				-
			</Badge>
		);
	}

	return (
		<Badge appearance="filled" color={departmentColors[department] ?? "subtle"}>
			{departmentLabels[department] ?? department}
		</Badge>
	);
};

export default SystemUserDepartmentBadge;
