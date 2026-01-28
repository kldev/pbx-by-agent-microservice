import {
	Divider,
	DrawerBody,
	DrawerHeader,
	DrawerHeaderTitle,
	Button,
	makeStyles,
	OverlayDrawer,
	Spinner,
	Text,
	tokens,
} from "@fluentui/react-components";
import { Dismiss24Regular } from "@fluentui/react-icons";
import type React from "react";
import { useEffect } from "react";
import { useGetSupervisorMonitorEntry } from "../../../../api/rcp/endpoints/rcp-supervisor/rcp-supervisor";
import { getPolishDayNameFull, toTimeFormat } from "../../utils";
import RcpStatusBadge from "../RcpStatusBadge";
import { CommentsSection } from "../shared";

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
	readOnlyBanner: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
		padding: tokens.spacingHorizontalM,
		backgroundColor: tokens.colorNeutralBackground3,
		borderRadius: tokens.borderRadiusMedium,
		color: tokens.colorNeutralForeground3,
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
	noDays: {
		textAlign: "center",
		padding: tokens.spacingVerticalL,
		color: tokens.colorNeutralForeground3,
	},
});

interface MonitoringDetailPanelProps {
	gid: string;
	onClose: () => void;
}

const MonitoringDetailPanel: React.FC<MonitoringDetailPanelProps> = ({
	gid,
	onClose,
}) => {
	const styles = useStyles();

	const getEntryMutation = useGetSupervisorMonitorEntry();
	const { data, isPending: isLoading } = getEntryMutation;

	useEffect(() => {
		getEntryMutation.mutate({ data: { gid } });
	}, [gid]);

	const entry = data?.status === 200 ? data.data : null;
	const days = entry?.days ?? [];

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
					{entry?.employeeFullName ?? "Podgląd wpisu"}
				</DrawerHeaderTitle>
			</DrawerHeader>
			<DrawerBody className={styles.body}>
				{isLoading ? (
					<div className={styles.loading}>
						<Spinner label="Ładowanie..." />
					</div>
				) : entry ? (
					<>
						<div className={styles.readOnlyBanner}>
							<Text>Tryb podglądu - monitorowanie postępu pracownika</Text>
						</div>

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
							<div className={styles.summaryItem}>
								<Text className={styles.label}>Wypełnione dni</Text>
								<Text className={styles.value}>{days.length}</Text>
							</div>
						</div>

						<Divider />

						<Text weight="semibold">Wpisy dzienne</Text>
						{days.length > 0 ? (
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
						) : (
							<div className={styles.noDays}>
								<Text>Pracownik nie wprowadził jeszcze żadnych wpisów</Text>
							</div>
						)}

						<Divider />

						<CommentsSection gid={gid} />
					</>
				) : null}
			</DrawerBody>
		</OverlayDrawer>
	);
};

export default MonitoringDetailPanel;
