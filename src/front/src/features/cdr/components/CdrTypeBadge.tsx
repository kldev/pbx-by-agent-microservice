import { Badge, Tooltip } from "@fluentui/react-components";
import type React from "react";

export interface CdrTypeBadgeBadgeProps {
	statusCode: string;
	statusDesc: string;
}

const CdrTypeBadge: React.FC<CdrTypeBadgeBadgeProps> = ({
	statusCode,
	statusDesc,
}) => {
	if (!statusCode) {
		return (
			<Badge appearance="outline" color="informative">
				Brak
			</Badge>
		);
	}
	return (
		<Tooltip content={statusDesc} relationship="description">
			<Badge appearance="outline" color="informative">
				{statusCode}
			</Badge>
		</Tooltip>
	);
};

export default CdrTypeBadge;
