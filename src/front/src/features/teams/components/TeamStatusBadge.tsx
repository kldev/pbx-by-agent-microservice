import type React from "react";
import { StatusBadge } from "../../../components/common";

export interface TeamStatusBadgeProps {
	isActive: boolean;
}

const TeamStatusBadge: React.FC<TeamStatusBadgeProps> = ({ isActive }) => {
	return (
		<StatusBadge
			status={isActive ? "success" : "default"}
			label={isActive ? "Aktywny" : "Nieaktywny"}
		/>
	);
};

export default TeamStatusBadge;
