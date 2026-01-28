import { makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";
import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";

import {
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import { TeamForm, type TeamFormData } from "../components";
import {
	useGetTeam,
	useUpdateTeam,
} from "../../../api/identity/endpoints/teams/teams";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		height: "100%",
	},
	content: {
		flex: 1,
		padding: tokens.spacingHorizontalL,
		paddingTop: tokens.spacingVerticalM,
		overflowY: "auto",
	},
});

const TeamEditPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { gid } = useParams<{ gid: string }>();
	const [initialData, setInitialData] = useState<Partial<TeamFormData> | null>(
		null,
	);

	const teamMutation = useGetTeam();
	const updateTeamMutation = useUpdateTeam();

	// biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
	useEffect(() => {
		if (gid) {
			teamMutation.mutate({ data: { gid } });
		}
	}, [gid]);

	useEffect(() => {
		if (teamMutation.isSuccess && teamMutation.data?.status === 200) {
			const team = teamMutation.data.data;
			setInitialData({
				code: team.code ?? "",
				name: team.name ?? "",
				structureId: team.structureId ?? 0,
				description: "",
				isActive: team.isActive ?? true,
			});
		}
	}, [teamMutation.isSuccess, teamMutation.data]);

	const handleSubmit = useCallback(
		(data: TeamFormData) => {
			if (!gid) return;

			updateTeamMutation.mutate(
				{
					gid,
					data: {
						code: data.code,
						name: data.name,
						structureId: data.structureId,
						isActive: data.isActive,
					},
				},
				{
					onSuccess: () => {
						navigate(`/teams/${gid}`);
					},
				},
			);
		},
		[gid, updateTeamMutation, navigate],
	);

	const handleCancel = useCallback(() => {
		if (gid) {
			navigate(`/teams/${gid}`);
		} else {
			navigate(ROUTES.TEAMS_LIST);
		}
	}, [gid, navigate]);

	const teamName =
		teamMutation.data?.status === 200
			? teamMutation.data.data.name || "Zespół"
			: "Zespół";

	if (teamMutation.isPending) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Edycja zespołu"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Zespoły", href: ROUTES.TEAMS_LIST },
						{ label: "Ładowanie..." },
					]}
				/>
				<div className={styles.content}>
					<LoadingSpinner label="Ładowanie danych zespołu..." />
				</div>
			</div>
		);
	}

	if (teamMutation.isError || teamMutation.data?.status !== 200) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Edycja zespołu"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Zespoły", href: ROUTES.TEAMS_LIST },
						{ label: "Błąd" },
					]}
				/>
				<div className={styles.content}>
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać danych zespołu."
						onRetry={() => {
							if (gid) {
								teamMutation.mutate({ data: { gid } });
							}
						}}
					/>
				</div>
			</div>
		);
	}

	return (
		<div className={styles.container}>
			<PageHeader
				title={`Edycja: ${teamName}`}
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Zespoły", href: ROUTES.TEAMS_LIST },
					{ label: teamName, href: `/teams/${gid}` },
					{ label: "Edycja" },
				]}
			/>
			<div className={styles.content}>
				{initialData && (
					<TeamForm
						initialData={initialData}
						onSubmit={handleSubmit}
						onCancel={handleCancel}
						isLoading={updateTeamMutation.isPending}
						submitLabel="Zapisz zmiany"
						isEditMode={true}
					/>
				)}
			</div>
		</div>
	);
};

export default TeamEditPage;
