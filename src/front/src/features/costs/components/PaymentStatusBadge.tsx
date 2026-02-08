import { Badge } from "@fluentui/react-components";
import type React from "react";

export interface PaymentStatusBadgeProps {
	wasPaid?: boolean;
}

const PaymentStatusBadge: React.FC<PaymentStatusBadgeProps> = ({ wasPaid }) => {
	if (wasPaid) {
		return (
			<Badge appearance="filled" color="success">
				Opłacona
			</Badge>
		);
	}
	return (
		<Badge appearance="filled" color="danger">
			Nieopłacona
		</Badge>
	);
};

export default PaymentStatusBadge;
