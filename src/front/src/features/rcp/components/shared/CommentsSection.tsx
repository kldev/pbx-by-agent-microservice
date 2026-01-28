import {
	Button,
	makeStyles,
	Spinner,
	Text,
	tokens,
} from "@fluentui/react-components";
import { AddRegular } from "@fluentui/react-icons";
import type React from "react";
import { useEffect, useState } from "react";
import { useGetComments } from "../../../../api/rcp/endpoints/rcp-comments/rcp-comments";
import AddCommentDialog from "./AddCommentDialog";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
	},
	header: {
		display: "flex",
		justifyContent: "space-between",
		alignItems: "center",
	},
	title: {
		fontWeight: tokens.fontWeightSemibold,
	},
	commentsList: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalS,
	},
	comment: {
		padding: tokens.spacingHorizontalM,
		backgroundColor: tokens.colorNeutralBackground2,
		borderRadius: tokens.borderRadiusMedium,
		borderLeft: `3px solid ${tokens.colorBrandStroke1}`,
	},
	commentHeader: {
		display: "flex",
		justifyContent: "space-between",
		alignItems: "center",
		marginBottom: tokens.spacingVerticalXS,
	},
	authorName: {
		fontWeight: tokens.fontWeightSemibold,
		fontSize: tokens.fontSizeBase300,
	},
	authorRole: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
		marginLeft: tokens.spacingHorizontalS,
	},
	commentDate: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
	commentContent: {
		fontSize: tokens.fontSizeBase300,
		whiteSpace: "pre-wrap",
	},
	emptyState: {
		textAlign: "center",
		padding: tokens.spacingVerticalM,
		color: tokens.colorNeutralForeground3,
	},
	loading: {
		display: "flex",
		justifyContent: "center",
		padding: tokens.spacingVerticalM,
	},
});

interface CommentsSectionProps {
	gid: string;
}

const CommentsSection: React.FC<CommentsSectionProps> = ({ gid }) => {
	const styles = useStyles();
	const [showDialog, setShowDialog] = useState(false);

	const getCommentsMutation = useGetComments();
	const { data, isPending: isLoading } = getCommentsMutation;

	// Fetch comments when gid changes
	useEffect(() => {
		getCommentsMutation.mutate({ data: { gid } });
	}, [gid]);

	const refetch = () => {
		getCommentsMutation.mutate({ data: { gid } });
	};

	const comments = data?.status === 200 ? data.data : [];

	const handleAddSuccess = () => {
		setShowDialog(false);
		refetch();
	};

	const formatDate = (dateString?: string) => {
		if (!dateString) return "";
		return new Date(dateString).toLocaleDateString("pl-PL", {
			day: "numeric",
			month: "short",
			year: "numeric",
			hour: "2-digit",
			minute: "2-digit",
		});
	};

	return (
		<div className={styles.container}>
			<div className={styles.header}>
				<Text className={styles.title}>Komentarze ({comments.length})</Text>
				<Button
					appearance="subtle"
					icon={<AddRegular />}
					onClick={() => setShowDialog(true)}
				>
					Dodaj
				</Button>
			</div>

			{isLoading ? (
				<div className={styles.loading}>
					<Spinner size="small" />
				</div>
			) : comments.length === 0 ? (
				<div className={styles.emptyState}>
					<Text>Brak komentarzy</Text>
				</div>
			) : (
				<div className={styles.commentsList}>
					{comments.map((comment) => (
						<div key={comment.gid} className={styles.comment}>
							<div className={styles.commentHeader}>
								<div>
									<Text className={styles.authorName}>
										{comment.authorName}
									</Text>
									<Text className={styles.authorRole}>
										({comment.authorRole})
									</Text>
								</div>
								<Text className={styles.commentDate}>
									{formatDate(comment.createdAt)}
								</Text>
							</div>
							<Text className={styles.commentContent}>{comment.content}</Text>
						</div>
					))}
				</div>
			)}

			{showDialog && (
				<AddCommentDialog
					gid={gid}
					onClose={() => setShowDialog(false)}
					onSuccess={handleAddSuccess}
				/>
			)}
		</div>
	);
};

export default CommentsSection;
