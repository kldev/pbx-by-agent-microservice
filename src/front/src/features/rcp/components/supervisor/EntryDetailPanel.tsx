import {
	Button,
	Divider,
	DrawerBody,
	DrawerHeader,
	DrawerHeaderTitle,
	makeStyles,
	OverlayDrawer,
	Spinner,
	Text,
	tokens,
} from "@fluentui/react-components";
import {
	ArrowUndoRegular,
	CheckmarkRegular,
	Dismiss24Regular,
	DismissRegular,
	SendRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useEffect, useState } from "react";
import { useGetEntryByGid } from "../../../../api/rcp/endpoints/rcp-supervisor/rcp-supervisor";
import { RcpTimeEntryStatus } from "../../../../api/rcp/models";
import { getPolishDayNameFull, toTimeFormat } from "../../utils";
import RcpStatusBadge from "../RcpStatusBadge";
import { CommentsSection } from "../shared";
import StatusChangeDialog from "./StatusChangeDialog";

const useStyles = makeStyles({
	body: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalL,
		padding: tokens.spacingHorizontalL,
	},
	summary: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalL,
		padding: tokens.spacingHorizontalM,
		backgroundColor: tokens.colorNeutralBackground2,
		borderRadius: tokens.borderRadiusMedium,
	},
	summaryItem: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXXS,
	},
	label: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
	value: {
		fontSize: tokens.fontSizeBase400,
		fontWeight: tokens.fontWeightSemibold,
	},
	actions: {
		display: "flex",
		gap: tokens.spacingHorizontalS,
		flexWrap: "wrap",
	},
	daysList: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXS,
	},
	dayItem: {
		display: "flex",
		justifyContent: "space-between",
		alignItems: "center",
		padding: tokens.spacingVerticalS,
		borderRadius: tokens.borderRadiusMedium,
		backgroundColor: tokens.colorNeutralBackground2,
	},
	dayItemWeekend: {
		backgroundColor: tokens.colorNeutralBackground3,
	},
	dayName: {
		fontWeight: tokens.fontWeightSemibold,
	},
	dayHours: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorBrandForeground1,
	},
	loading: {
		display: "flex",
		justifyContent: "center",
		padding: tokens.spacingVerticalXXL,
	},
});

interface EntryDetailPanelProps {
	gid: string;
	onClose: () => void;
	onStatusChange: () => void;
	mode: "supervisor" | "payroll";
}

type DialogAction = "approve" | "reject" | "toSettlement" | "return" | null;

const EntryDetailPanel: React.FC<EntryDetailPanelProps> = ({
	gid,
	onClose,
	onStatusChange,
	mode,
}) => {
	const styles = useStyles();
	const [dialogAction, setDialogAction] = useState<DialogAction>(null);

	const getEntryMutation = useGetEntryByGid();
	const { data, isPending: isLoading } = getEntryMutation;

	// Fetch data when gid changes
	useEffect(() => {
		getEntryMutation.mutate({ data: { gid } });
	}, [gid]);

	const entry = data?.status === 200 ? data.data : null;
	const days = entry?.days ?? [];
	const status = entry?.status;

	const canApprove = status === RcpTimeEntryStatus.Submitted;
	const canReject = status === RcpTimeEntryStatus.Submitted;
	const canToSettlement = status === RcpTimeEntryStatus.Approved;
	const canReturn =
		mode === "payroll" &&
		(status === RcpTimeEntryStatus.Approved ||
			status === RcpTimeEntryStatus.Settlement);

	const handleDialogClose = () => {
		setDialogAction(null);
	};

	const handleDialogSuccess = () => {
		setDialogAction(null);
		onStatusChange();
	};

	return (
		<OverlayDrawer
			open
			position="end"
			size="medium"
			onOpenChange={(_, data) => !data.open && onClose()}
		>
			<DrawerHeader>
				<DrawerHeaderTitle
					action={
						<Button
							appearance="subtle"
							icon={<Dismiss24Regular />}
							onClick={onClose}
						/>
					}
				>
					{entry?.employeeFullName ?? "Szczegóły wpisu"}
				</DrawerHeaderTitle>
			</DrawerHeader>
			<DrawerBody className={styles.body}>
				{isLoading ? (
					<div className={styles.loading}>
						<Spinner label="Ładowanie..." />
					</div>
				) : entry ? (
					<>
						<div className={styles.summary}>
							<div className={styles.summaryItem}>
								<Text className={styles.label}>Suma godzin</Text>
								<Text className={styles.value}>
									{toTimeFormat(entry.totalMinutes ?? 0)}
								</Text>
							</div>
							<div className={styles.summaryItem}>
								<Text className={styles.label}>Status</Text>
								<RcpStatusBadge status={entry.status} />
							</div>
						</div>

						<div className={styles.actions}>
							{mode === "supervisor" && canApprove && (
								<Button
									appearance="primary"
									icon={<CheckmarkRegular />}
									onClick={() => setDialogAction("approve")}
								>
									Zatwierdź
								</Button>
							)}
							{mode === "supervisor" && canReject && (
								<Button
									appearance="secondary"
									icon={<DismissRegular />}
									onClick={() => setDialogAction("reject")}
								>
									Odrzuć
								</Button>
							)}
							{mode === "supervisor" && canToSettlement && (
								<Button
									appearance="primary"
									icon={<SendRegular />}
									onClick={() => setDialogAction("toSettlement")}
								>
									Do rozliczenia
								</Button>
							)}
							{canReturn && (
								<Button
									appearance="secondary"
									icon={<ArrowUndoRegular />}
									onClick={() => setDialogAction("return")}
								>
									Zwróć do poprawy
								</Button>
							)}
						</div>

						<Divider />

						<Text weight="semibold">Wpisy dzienne</Text>
						<div className={styles.daysList}>
							{days.map((day) => {
								const date = new Date(day.workDate ?? "");
								const isWeekend = date.getDay() === 0 || date.getDay() === 6;
								return (
									<div
										key={day.gid}
										className={`${styles.dayItem} ${isWeekend ? styles.dayItemWeekend : ""}`}
									>
										<div>
											<Text className={styles.dayName}>
												{getPolishDayNameFull(date.getDay())},{" "}
												{date.toLocaleDateString("pl-PL")}
											</Text>
											<br />
											<Text>
												{day.startTime} - {day.endTime}
											</Text>
										</div>
										<Text className={styles.dayHours}>
											{toTimeFormat(day.workMinutes ?? 0)}
										</Text>
									</div>
								);
							})}
						</div>

						<Divider />

						<CommentsSection gid={gid} />
					</>
				) : null}
			</DrawerBody>

			{dialogAction && (
				<StatusChangeDialog
					gid={gid}
					action={dialogAction}
					onClose={handleDialogClose}
					onSuccess={handleDialogSuccess}
				/>
			)}
		</OverlayDrawer>
	);
};

export default EntryDetailPanel;
