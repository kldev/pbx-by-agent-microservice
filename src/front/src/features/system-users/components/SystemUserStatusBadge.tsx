import type React from "react";
import { StatusBadge } from "../../../components/common";

export interface SystemUserStatusBadgeProps {
	isActive: boolean;
}

const SystemUserStatusBadge: React.FC<SystemUserStatusBadgeProps> = ({
	isActive,
}) => {
	return (
		<StatusBadge
			status={isActive ? "success" : "default"}
			label={isActive ? "Aktywny" : "Nieaktywny"}
		/>
	);
};

export default SystemUserStatusBadge;
