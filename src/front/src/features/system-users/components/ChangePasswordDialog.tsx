import {
	Button,
	Dialog,
	DialogActions,
	DialogBody,
	DialogContent,
	DialogSurface,
	DialogTitle,
	Field,
	Input,
	makeStyles,
	Spinner,
	tokens,
} from "@fluentui/react-components";
import { DismissRegular, KeyRegular } from "@fluentui/react-icons";
import type React from "react";
import { Controller, useForm } from "react-hook-form";
import { useChangeAppUserPassword } from "../../../api/identity/endpoints/app-users/app-users";

const useStyles = makeStyles({
	form: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
	},
});

interface ChangePasswordFormData {
	newPassword: string;
	confirmPassword: string;
}

export interface ChangePasswordDialogProps {
	open: boolean;
	employeeGid: string | null;
	employeeName: string;
	onOpenChange: (open: boolean) => void;
	onSuccess?: () => void;
}

const ChangePasswordDialog: React.FC<ChangePasswordDialogProps> = ({
	open,
	employeeGid,
	employeeName,
	onOpenChange,
	onSuccess,
}) => {
	const styles = useStyles();
	const changePasswordMutation = useChangeAppUserPassword();

	const {
		control,
		handleSubmit,
		reset,
		watch,
		formState: { errors },
	} = useForm<ChangePasswordFormData>({
		defaultValues: {
			newPassword: "",
			confirmPassword: "",
		},
	});

	const newPassword = watch("newPassword");

	const handleClose = () => {
		reset();
		onOpenChange(false);
	};

	const onSubmit = (data: ChangePasswordFormData) => {
		if (!employeeGid) return;

		changePasswordMutation.mutate(
			{
				gid: employeeGid,
				data: {
					newPassword: data.newPassword,
				},
			},
			{
				onSuccess: () => {
					reset();
					onOpenChange(false);
					onSuccess?.();
				},
			},
		);
	};

	return (
		<Dialog open={open} onOpenChange={(_, data) => onOpenChange(data.open)}>
			<DialogSurface>
				<form onSubmit={handleSubmit(onSubmit)}>
					<DialogBody>
						<DialogTitle>Zmiana hasła</DialogTitle>
						<DialogContent>
							<p style={{ marginBottom: tokens.spacingVerticalM }}>
								Ustaw nowe hasło dla użytkownika:{" "}
								<strong>{employeeName}</strong>
							</p>
							<div className={styles.form}>
								<Field
									label="Nowe hasło *"
									validationState={errors.newPassword ? "error" : "none"}
									validationMessage={errors.newPassword?.message}
								>
									<Controller
										name="newPassword"
										control={control}
										rules={{
											required: "Hasło jest wymagane",
											minLength: {
												value: 6,
												message: "Hasło musi mieć co najmniej 6 znaków",
											},
										}}
										render={({ field }) => (
											<Input
												{...field}
												type="password"
												placeholder="Wprowadź nowe hasło"
											/>
										)}
									/>
								</Field>

								<Field
									label="Potwierdź hasło *"
									validationState={errors.confirmPassword ? "error" : "none"}
									validationMessage={errors.confirmPassword?.message}
								>
									<Controller
										name="confirmPassword"
										control={control}
										rules={{
											required: "Potwierdzenie hasła jest wymagane",
											validate: (value) =>
												value === newPassword || "Hasła muszą być identyczne",
										}}
										render={({ field }) => (
											<Input
												{...field}
												type="password"
												placeholder="Potwierdź nowe hasło"
											/>
										)}
									/>
								</Field>
							</div>
						</DialogContent>
						<DialogActions>
							<Button
								appearance="secondary"
								icon={<DismissRegular />}
								onClick={handleClose}
								disabled={changePasswordMutation.isPending}
							>
								Anuluj
							</Button>
							<Button
								appearance="primary"
								type="submit"
								icon={
									changePasswordMutation.isPending ? (
										<Spinner size="tiny" />
									) : (
										<KeyRegular />
									)
								}
								disabled={changePasswordMutation.isPending}
							>
								Zmień hasło
							</Button>
						</DialogActions>
					</DialogBody>
				</form>
			</DialogSurface>
		</Dialog>
	);
};

export default ChangePasswordDialog;
