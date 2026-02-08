import { makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";
import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";

import {
	useGetRuleByGid,
	useUpdateRule,
} from "../../../api/answerrule/endpoints/answering-rules/answering-rules";
import {
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import AnsweringRuleForm from "../components/AnsweringRuleForm";
import type { AnsweringRuleFormData } from "../components/AnsweringRuleForm";
import { AnsweringRuleAction } from "../../../api/answerrule/models";

const useStyles = makeStyles({
	container: { display: "flex", flexDirection: "column", height: "100%" },
	content: {
		flex: 1,
		padding: tokens.spacingHorizontalL,
		paddingTop: tokens.spacingVerticalM,
		overflowY: "auto",
	},
});

const AnsweringRuleEditPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { gid } = useParams<{ gid: string }>();
	const [initialData, setInitialData] =
		useState<Partial<AnsweringRuleFormData> | null>(null);

	const ruleQuery = useGetRuleByGid(gid ?? "");
	const updateRuleMutation = useUpdateRule();

	useEffect(() => {
		if (ruleQuery.isSuccess && ruleQuery.data?.status === 200) {
			const rule = ruleQuery.data.data;
			setInitialData({
				sipAccountGid: rule.sipAccountGid ?? "",
				name: rule.name ?? "",
				description: rule.description ?? "",
				priority: rule.priority ?? 100,
				isEnabled: rule.isEnabled ?? true,
				actionType: rule.actionType ?? AnsweringRuleAction.Voicemail,
				actionTarget: rule.actionTarget ?? "",
				voicemailBoxGid: rule.voicemailBoxGid ?? "",
				voiceMessageGid: rule.voiceMessageGid ?? "",
				sendEmailNotification: rule.sendEmailNotification ?? false,
				notificationEmail: rule.notificationEmail ?? "",
				timeSlots: rule.timeSlots ?? [],
			});
		}
	}, [ruleQuery.isSuccess, ruleQuery.data]);

	const handleSubmit = useCallback(
		(data: AnsweringRuleFormData) => {
			if (!gid) return;
			updateRuleMutation.mutate(
				{
					gid,
					data: {
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
					onSuccess: () => {
						navigate(`/answering-rules/${gid}`);
					},
				},
			);
		},
		[gid, updateRuleMutation, navigate],
	);

	const handleCancel = useCallback(() => {
		if (gid) {
			navigate(`/answering-rules/${gid}`);
		} else {
			navigate(ROUTES.ANSWERING_RULES_LIST);
		}
	}, [gid, navigate]);

	const ruleName =
		ruleQuery.data?.status === 200
			? ruleQuery.data.data.name || "Reguła"
			: "Reguła";

	if (ruleQuery.isLoading) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Edycja reguły"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{
							label: "Reguły odpowiadania",
							href: ROUTES.ANSWERING_RULES_LIST,
						},
						{ label: "Ładowanie..." },
					]}
				/>
				<div className={styles.content}>
					<LoadingSpinner label="Ładowanie danych reguły..." />
				</div>
			</div>
		);
	}

	if (ruleQuery.isError || ruleQuery.data?.status !== 200) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Edycja reguły"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{
							label: "Reguły odpowiadania",
							href: ROUTES.ANSWERING_RULES_LIST,
						},
						{ label: "Błąd" },
					]}
				/>
				<div className={styles.content}>
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać danych reguły."
						onRetry={() => ruleQuery.refetch()}
					/>
				</div>
			</div>
		);
	}

	return (
		<div className={styles.container}>
			<PageHeader
				title={`Edycja: ${ruleName}`}
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{
						label: "Reguły odpowiadania",
						href: ROUTES.ANSWERING_RULES_LIST,
					},
					{ label: ruleName, href: `/answering-rules/${gid}` },
					{ label: "Edycja" },
				]}
			/>
			<div className={styles.content}>
				{initialData && (
					<AnsweringRuleForm
						initialData={initialData}
						onSubmit={handleSubmit}
						onCancel={handleCancel}
						isLoading={updateRuleMutation.isPending}
						submitLabel="Zapisz zmiany"
						isEditMode={true}
					/>
				)}
			</div>
		</div>
	);
};

export default AnsweringRuleEditPage;
