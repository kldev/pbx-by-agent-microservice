import { makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";
import { useCallback } from "react";
import { useNavigate } from "react-router";

import { useCreateRule } from "../../../api/answerrule/endpoints/answering-rules/answering-rules";
import { PageHeader } from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import AnsweringRuleForm from "../components/AnsweringRuleForm";
import type { AnsweringRuleFormData } from "../components/AnsweringRuleForm";

const useStyles = makeStyles({
	container: { display: "flex", flexDirection: "column", height: "100%" },
	content: {
		flex: 1,
		padding: tokens.spacingHorizontalL,
		paddingTop: tokens.spacingVerticalM,
		overflowY: "auto",
	},
});

const AnsweringRuleCreatePage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const createRuleMutation = useCreateRule();

	const handleSubmit = useCallback(
		(data: AnsweringRuleFormData) => {
			createRuleMutation.mutate(
				{
					data: {
						sipAccountGid: data.sipAccountGid,
						name: data.name,
						description: data.description || undefined,
						priority: data.priority,
						isEnabled: data.isEnabled,
						actionType: data.actionType,
						actionTarget: data.actionTarget || undefined,
						voicemailBoxGid: data.voicemailBoxGid || undefined,
						voiceMessageGid: data.voiceMessageGid || undefined,
						sendEmailNotification: data.sendEmailNotification,
						notificationEmail: data.notificationEmail || undefined,
						timeSlots: data.timeSlots,
					},
				},
				{
					onSuccess: (response) => {
						if (response.status === 200 && response.data.gid) {
							navigate(`/answering-rules/${response.data.gid}`);
						} else {
							navigate(ROUTES.ANSWERING_RULES_LIST);
						}
					},
				},
			);
		},
		[createRuleMutation, navigate],
	);

	const handleCancel = useCallback(() => {
		navigate(ROUTES.ANSWERING_RULES_LIST);
	}, [navigate]);

	return (
		<div className={styles.container}>
			<PageHeader
				title="Nowa reguła"
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{
						label: "Reguły odpowiadania",
						href: ROUTES.ANSWERING_RULES_LIST,
					},
					{ label: "Nowa reguła" },
				]}
			/>
			<div className={styles.content}>
				<AnsweringRuleForm
					onSubmit={handleSubmit}
					onCancel={handleCancel}
					isLoading={createRuleMutation.isPending}
					submitLabel="Utwórz regułę"
					isEditMode={false}
				/>
			</div>
		</div>
	);
};

export default AnsweringRuleCreatePage;
