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
import { useAddComment } from "../../../../api/rcp/endpoints/rcp-comments/rcp-comments";

const useStyles = makeStyles({
	closeButton: {
		minWidth: "auto",
		padding: tokens.spacingHorizontalXS,
	},
});

interface AddCommentDialogProps {
	gid: string;
	onClose: () => void;
	onSuccess: () => void;
}

const AddCommentDialog: React.FC<AddCommentDialogProps> = ({
	gid,
	onClose,
	onSuccess,
}) => {
	const styles = useStyles();
	const [content, setContent] = useState("");

	const addCommentMutation = useAddComment();

	const handleSubmit = async () => {
		try {
			await addCommentMutation.mutateAsync({
				gid,
				data: { content },
			});
			onSuccess();
		} catch (error) {
			console.error("Failed to add comment:", error);
		}
	};

	const isLoading = addCommentMutation.isPending;

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
						Dodaj komentarz
					</DialogTitle>
					<DialogContent>
						<Field label="Treść komentarza">
							<Textarea
								value={content}
								onChange={(_, data) => setContent(data.value)}
								placeholder="Wpisz komentarz..."
								rows={4}
							/>
						</Field>
					</DialogContent>
					<DialogActions>
						<Button
							appearance="primary"
							onClick={handleSubmit}
							disabled={isLoading || !content.trim()}
						>
							{isLoading ? "Zapisywanie..." : "Dodaj"}
						</Button>
					</DialogActions>
				</DialogBody>
			</DialogSurface>
		</Dialog>
	);
};

export default AddCommentDialog;
