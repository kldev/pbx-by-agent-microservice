import { Badge } from "@fluentui/react-components";
import type React from "react";
import { RcpTimeEntryStatus } from "../../../api/rcp/models";
import { timeEntryStatusLabels } from "../../../constants/enumLabels";

interface RcpStatusBadgeProps {
	status?: RcpTimeEntryStatus;
}

const statusColors: Record<
	RcpTimeEntryStatus,
	"subtle" | "warning" | "success" | "danger" | "informative"
> = {
	[RcpTimeEntryStatus.Draft]: "subtle",
	[RcpTimeEntryStatus.Submitted]: "warning",
	[RcpTimeEntryStatus.Approved]: "success",
	[RcpTimeEntryStatus.Correction]: "danger",
	[RcpTimeEntryStatus.Settlement]: "informative",
};

const RcpStatusBadge: React.FC<RcpStatusBadgeProps> = ({ status }) => {
	if (!status) {
		return (
			<Badge appearance="outline" color="subtle">
				Brak
			</Badge>
		);
	}

	return (
		<Badge appearance="filled" color={statusColors[status]}>
			{timeEntryStatusLabels[status]}
		</Badge>
	);
};

export default RcpStatusBadge;
