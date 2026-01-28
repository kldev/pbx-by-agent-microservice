import { Badge, type BadgeProps } from "@fluentui/react-components";
import type React from "react";

export type StatusType = "success" | "warning" | "error" | "info" | "default";

export interface StatusBadgeProps
	extends Omit<BadgeProps, "color" | "appearance"> {
	status: StatusType;
	label: string;
}

const statusColorMap: Record<StatusType, BadgeProps["color"]> = {
	success: "success",
	warning: "warning",
	error: "danger",
	info: "informative",
	default: "subtle",
};

const StatusBadge: React.FC<StatusBadgeProps> = ({
	status,
	label,
	...props
}) => {
	return (
		<Badge {...props} color={statusColorMap[status]} appearance="filled">
			{label}
		</Badge>
	);
};

export default StatusBadge;
