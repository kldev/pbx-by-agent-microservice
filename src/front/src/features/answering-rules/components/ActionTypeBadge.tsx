import { Badge, makeStyles, tokens } from "@fluentui/react-components";
import {
	CallForwardRegular,
	PeopleRegular,
	Speaker0Regular,
	VoicemailRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { AnsweringRuleAction } from "../../../api/answerrule/models";

const ACTION_TYPE_CONFIG: Record<
	AnsweringRuleAction,
	{ label: string; color: "brand" | "success" | "warning" | "informative"; icon: React.ReactNode }
> = {
	Voicemail: {
		label: "Poczta głosowa",
		color: "brand",
		icon: <VoicemailRegular />,
	},
	Redirect: {
		label: "Przekierowanie",
		color: "success",
		icon: <CallForwardRegular />,
	},
	RedirectToGroup: {
		label: "Przekierowanie do grupy",
		color: "warning",
		icon: <PeopleRegular />,
	},
	DisconnectWithVoicemessage: {
		label: "Komunikat głosowy",
		color: "informative",
		icon: <Speaker0Regular />,
	},
};

const useStyles = makeStyles({
	badge: {
		display: "inline-flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalXS,
	},
});

interface ActionTypeBadgeProps {
	actionType: AnsweringRuleAction;
}

const ActionTypeBadge: React.FC<ActionTypeBadgeProps> = ({ actionType }) => {
	const styles = useStyles();
	const config = ACTION_TYPE_CONFIG[actionType];

	if (!config) return <Badge appearance="outline">{actionType}</Badge>;

	return (
		<Badge appearance="tint" color={config.color} className={styles.badge}>
			{config.icon}
			{config.label}
		</Badge>
	);
};

export default ActionTypeBadge;
