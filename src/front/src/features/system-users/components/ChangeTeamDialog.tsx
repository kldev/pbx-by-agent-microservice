import {
	Button,
	Dialog,
	DialogActions,
	DialogBody,
	DialogContent,
	DialogSurface,
	DialogTitle,
	Dropdown,
	Field,
	Option,
	Spinner,
	makeStyles,
	tokens,
} from "@fluentui/react-components";
import { DismissRegular, PeopleTeamRegular } from "@fluentui/react-icons";
import {
	forwardRef,
	useCallback,
	useEffect,
	useImperativeHandle,
	useState,
} from "react";
import { Controller, useForm } from "react-hook-form";
import { useUpdateAppUser } from "../../../api/identity/endpoints/app-users/app-users";
import { useGetTeamList } from "../../../api/identity/endpoints/teams/teams";
import type {
	AppUserResponse,
	TeamResponse,
} from "../../../api/identity/models";
import type { ChangeTeamDialogCommand } from "./ChangeTeamDialogCommand";

const useStyles = makeStyles({
	form: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
	},
	infoText: {
		marginBottom: tokens.spacingVerticalM,
	},
});

interface ChangeTeamFormData {
	teamId: number | null;
}

export interface ChangeTeamDialogProps {
	onSuccess?: () => void;
}

const ChangeTeamDialog = forwardRef<
	ChangeTeamDialogCommand,
	ChangeTeamDialogProps
>(({ onSuccess }, ref) => {
	const styles = useStyles();

	const [isOpen, setIsOpen] = useState(false);
	const [user, setUser] = useState<AppUserResponse | null>(null);
	const [teams, setTeams] = useState<TeamResponse[]>([]);

	const updateAppUserMutation = useUpdateAppUser();
	const getTeamListMutation = useGetTeamList();

	const { control, handleSubmit, reset, watch } = useForm<ChangeTeamFormData>({
		defaultValues: {
			teamId: null,
		},
	});

	useImperativeHandle(ref, () => ({
		open: (userData) => {
			setUser(userData);
			reset({ teamId: userData.teamId ?? null });
			setIsOpen(true);

			if (userData.structureId) {
				getTeamListMutation.mutate({
					data: {
						structureId: userData.structureId,
						isActive: true,
						pageNumber: 1,
						pageSize: 100,
					},
				});
			}
		},
	}));

	useEffect(() => {
		if (getTeamListMutation.data?.status === 200) {
			setTeams(getTeamListMutation.data.data.items ?? []);
		}
	}, [getTeamListMutation.data]);

	const handleClose = useCallback(() => {
		setIsOpen(false);
		setUser(null);
		setTeams([]);
		reset({ teamId: null });
	}, [reset]);

	const onSubmit = useCallback(
		(data: ChangeTeamFormData) => {
			if (!user?.gid) return;

			updateAppUserMutation.mutate(
				{
					gid: user.gid,
					data: {
						firstName: user.firstName ?? "",
						lastName: user.lastName ?? "",
						email: user.email ?? "",
						teamId: data.teamId,
					},
				},
				{
					onSuccess: () => {
						handleClose();
						onSuccess?.();
					},
				},
			);
		},
		[user, updateAppUserMutation, handleClose, onSuccess],
	);

	const selectedTeamId = watch("teamId");
	const selectedTeam = teams.find((t) => t.id === selectedTeamId);
	const userFullName =
		[user?.firstName, user?.lastName].filter(Boolean).join(" ") || "Użytkownik";

	if (user && !user.structureId) {
		return (
			<Dialog
				open={isOpen}
				onOpenChange={(_, data) => !data.open && handleClose()}
			>
				<DialogSurface>
					<DialogBody>
						<DialogTitle>Zmiana zespołu</DialogTitle>
						<DialogContent>
							<p>
								Użytkownik <strong>{userFullName}</strong> nie ma przypisanej
								struktury. Najpierw przypisz użytkownika do struktury, aby móc
								przypisać zespół.
							</p>
						</DialogContent>
						<DialogActions>
							<Button appearance="secondary" onClick={handleClose}>
								Zamknij
							</Button>
						</DialogActions>
					</DialogBody>
				</DialogSurface>
			</Dialog>
		);
	}

	return (
		<Dialog
			open={isOpen}
			onOpenChange={(_, data) => !data.open && handleClose()}
		>
			<DialogSurface>
				<form onSubmit={handleSubmit(onSubmit)}>
					<DialogBody>
						<DialogTitle>Zmiana zespołu</DialogTitle>
						<DialogContent>
							<p className={styles.infoText}>
								Zmień zespół dla użytkownika: <strong>{userFullName}</strong>
							</p>
							<div className={styles.form}>
								<Field label="Zespół">
									<Controller
										name="teamId"
										control={control}
										render={({ field }) => (
											<Dropdown
												placeholder={
													getTeamListMutation.isPending
														? "Ładowanie..."
														: "Wybierz zespół..."
												}
												value={selectedTeam?.name ?? "Brak zespołu"}
												selectedOptions={
													field.value ? [String(field.value)] : []
												}
												onOptionSelect={(_, data) => {
													const newValue = data.optionValue;
													field.onChange(newValue ? Number(newValue) : null);
												}}
												disabled={getTeamListMutation.isPending}
											>
												<Option key="none" value="" text="Brak zespołu">
													Brak zespołu
												</Option>
												{teams.map((team) => (
													<Option
														key={team.id}
														value={String(team.id)}
														text={team.name ?? ""}
													>
														{team.name}
														{team.code && (
															<span style={{ color: "gray", marginLeft: 8 }}>
																({team.code})
															</span>
														)}
													</Option>
												))}
											</Dropdown>
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
								disabled={updateAppUserMutation.isPending}
							>
								Anuluj
							</Button>
							<Button
								appearance="primary"
								type="submit"
								icon={
									updateAppUserMutation.isPending ? (
										<Spinner size="tiny" />
									) : (
										<PeopleTeamRegular />
									)
								}
								disabled={updateAppUserMutation.isPending}
							>
								Zapisz
							</Button>
						</DialogActions>
					</DialogBody>
				</form>
			</DialogSurface>
		</Dialog>
	);
});

ChangeTeamDialog.displayName = "ChangeTeamDialog";

export default ChangeTeamDialog;
