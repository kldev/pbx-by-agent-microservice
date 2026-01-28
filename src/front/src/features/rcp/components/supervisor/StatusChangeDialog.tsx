import {
	Button,
	Dialog,
	DialogActions,
	DialogBody,
	DialogContent,
	DialogSurface,
	DialogTitle,
	Field,
	makeStyles,
	Textarea,
	tokens,
} from "@fluentui/react-components";
import { Dismiss24Regular } from "@fluentui/react-icons";
import type React from "react";
import { useState } from "react";
import {
	useApproveEntry,
	useRejectEntry,
	useToSettlement,
} from "../../../../api/rcp/endpoints/rcp-supervisor/rcp-supervisor";
import { useReturnForCorrection } from "../../../../api/rcp/endpoints/rcp-payroll/rcp-payroll";
import { DialogHint } from "../shared";

const useStyles = makeStyles({
	titleRow: {
		display: "flex",
		justifyContent: "space-between",
		alignItems: "center",
		width: "100%",
	},
	closeButton: {
		minWidth: "auto",
		padding: tokens.spacingHorizontalXS,
	},
});

interface StatusChangeDialogProps {
	gid: string;
	action: "approve" | "reject" | "toSettlement" | "return";
	onClose: () => void;
	onSuccess: () => void;
}

const ACTION_CONFIG = {
	approve: {
		title: "Zatwierdź wpis",
		description: "Czy na pewno chcesz zatwierdzić ten wpis?",
		buttonLabel: "Zatwierdź",
		buttonAppearance: "primary" as const,
		hint: "Po zatwierdzeniu pracownik nie będzie mógł już edytować wpisów za ten miesiąc.",
	},
	reject: {
		title: "Odrzuć wpis",
		description:
			"Wpis zostanie zwrócony do poprawy. Dodaj komentarz dla pracownika.",
		buttonLabel: "Odrzuć",
		buttonAppearance: "secondary" as const,
		hint: 'Wpisz konkretnie co jest nieprawidłowe, np. "Brak wpisów za 15-16.12" lub "Nadgodziny 20.12 nie były uzgodnione".',
	},
	toSettlement: {
		title: "Przekaż do rozliczenia",
		description: "Wpis zostanie przekazany do działu kadr.",
		buttonLabel: "Przekaż",
		buttonAppearance: "primary" as const,
		hint: "Kadry użyją tych danych do naliczenia wynagrodzenia. Upewnij się, że wszystko jest poprawne.",
	},
	return: {
		title: "Zwróć do poprawy",
		description: "Wpis zostanie zwrócony pracownikowi do poprawy.",
		buttonLabel: "Zwróć",
		buttonAppearance: "secondary" as const,
		hint: 'Wpisz konkretnie co jest nieprawidłowe, np. "Suma godzin nie zgadza się z listą obecności".',
	},
};

const StatusChangeDialog: React.FC<StatusChangeDialogProps> = ({
	gid,
	action,
	onClose,
	onSuccess,
}) => {
	const styles = useStyles();
	const [comment, setComment] = useState("");

	const approveMutation = useApproveEntry();
	const rejectMutation = useRejectEntry();
	const toSettlementMutation = useToSettlement();
	const returnMutation = useReturnForCorrection();

	const config = ACTION_CONFIG[action];

	const isLoading =
		approveMutation.isPending ||
		rejectMutation.isPending ||
		toSettlementMutation.isPending ||
		returnMutation.isPending;

	const handleSubmit = async () => {
		try {
			const commentValue = comment || null;

			switch (action) {
				case "approve":
					await approveMutation.mutateAsync({
						gid,
						data: { comment: commentValue },
					});
					break;
				case "reject":
					await rejectMutation.mutateAsync({
						gid,
						data: { comment: commentValue },
					});
					break;
				case "toSettlement":
					await toSettlementMutation.mutateAsync({
						gid,
						data: { comment: commentValue },
					});
					break;
				case "return":
					// useReturnRcpForCorrection expects { data: { gid, comment } }
					await returnMutation.mutateAsync({
						data: { gid, comment: commentValue },
					});
					break;
			}

			onSuccess();
		} catch (error) {
			console.error("Failed to change status:", error);
		}
	};

	const requiresComment = action === "reject" || action === "return";

	return (
		<Dialog open onOpenChange={(_, data) => !data.open && onClose()}>
			<DialogSurface>
				<DialogBody>
					<DialogTitle
						action={
							<Button
								appearance="subtle"
								icon={<Dismiss24Regular />}
								onClick={onClose}
								className={styles.closeButton}
								disabled={isLoading}
							/>
						}
					>
						{config.title}
					</DialogTitle>
					<DialogContent>
						<DialogHint>{config.hint}</DialogHint>
						<p>{config.description}</p>
						<Field
							label={
								requiresComment
									? "Komentarz (wymagany)"
									: "Komentarz (opcjonalnie)"
							}
							style={{ marginTop: "16px" }}
						>
							<Textarea
								value={comment}
								onChange={(_, data) => setComment(data.value)}
								placeholder="Dodaj komentarz..."
								rows={3}
							/>
						</Field>
					</DialogContent>
					<DialogActions>
						<Button
							appearance={config.buttonAppearance}
							onClick={handleSubmit}
							disabled={isLoading || (requiresComment && !comment.trim())}
						>
							{isLoading ? "Przetwarzanie..." : config.buttonLabel}
						</Button>
					</DialogActions>
				</DialogBody>
			</DialogSurface>
		</Dialog>
	);
};

export default StatusChangeDialog;
