import { makeStyles, tokens } from "@fluentui/react-components";
import type React from "react";
import { useCallback } from "react";
import { useNavigate } from "react-router";

import { PageHeader } from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import { TeamForm, type TeamFormData } from "../components";
import { useCreateTeam } from "../../../api/identity/endpoints/teams/teams";

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

const TeamCreatePage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const createTeamMutation = useCreateTeam();

	const handleSubmit = useCallback(
		(data: TeamFormData) => {
			createTeamMutation.mutate(
				{
					data: {
						code: data.code,
						name: data.name,
						structureId: data.structureId,
					},
				},
				{
					onSuccess: (response) => {
						if (response.status === 201 && response.data.gid) {
							navigate(`/teams/${response.data.gid}`);
						} else {
							navigate(ROUTES.TEAMS_LIST);
						}
					},
				},
			);
		},
		[createTeamMutation, navigate],
	);

	const handleCancel = useCallback(() => {
		navigate(ROUTES.TEAMS_LIST);
	}, [navigate]);

	return (
		<div className={styles.container}>
			<PageHeader
				title="Nowy zespół"
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Zespoły", href: ROUTES.TEAMS_LIST },
					{ label: "Nowy zespół" },
				]}
			/>
			<div className={styles.content}>
				<TeamForm
					onSubmit={handleSubmit}
					onCancel={handleCancel}
					isLoading={createTeamMutation.isPending}
					submitLabel="Utwórz zespół"
					isEditMode={false}
				/>
			</div>
		</div>
	);
};

export default TeamCreatePage;
