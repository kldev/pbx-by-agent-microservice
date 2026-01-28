import {
	Button,
	Dialog,
	DialogActions,
	DialogBody,
	DialogContent,
	DialogSurface,
	DialogTitle,
	DialogTrigger,
} from "@fluentui/react-components";
import type React from "react";

export interface ConfirmDialogProps {
	open: boolean;
	onOpenChange: (open: boolean) => void;
	title: string;
	message: string;
	confirmLabel?: string;
	cancelLabel?: string;
	onConfirm: () => void;
	danger?: boolean;
	loading?: boolean;
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
	open,
	onOpenChange,
	title,
	message,
	confirmLabel = "Potwierdź",
	cancelLabel = "Anuluj",
	onConfirm,
	danger = false,
	loading = false,
}) => {
	const handleConfirm = () => {
		onConfirm();
	};

	return (
		<Dialog open={open} onOpenChange={(_, data) => onOpenChange(data.open)}>
			<DialogSurface>
				<DialogBody>
					<DialogTitle>{title}</DialogTitle>
					<DialogContent>{message}</DialogContent>
					<DialogActions>
						<DialogTrigger disableButtonEnhancement>
							<Button appearance="secondary" disabled={loading}>
								{cancelLabel}
							</Button>
						</DialogTrigger>
						<Button
							appearance={danger ? "primary" : "primary"}
							onClick={handleConfirm}
							disabled={loading}
							style={danger ? { backgroundColor: "#d13438" } : undefined}
						>
							{loading ? "Ładowanie..." : confirmLabel}
						</Button>
					</DialogActions>
				</DialogBody>
			</DialogSurface>
		</Dialog>
	);
};

export default ConfirmDialog;
