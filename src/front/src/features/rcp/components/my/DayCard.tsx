import {
	makeStyles,
	mergeClasses,
	Text,
	tokens,
} from "@fluentui/react-components";
import { AddRegular, NoteRegular } from "@fluentui/react-icons";
import type React from "react";
import type { DayEntryResponse } from "../../../../api/rcp/models";
import type { CalendarDay } from "../../utils";
import { getPolishDayNameFull, toTimeFormat } from "../../utils";

const useStyles = makeStyles({
	card: {
		display: "flex",
		alignItems: "center",
		justifyContent: "space-between",
		padding: tokens.spacingHorizontalM,
		borderRadius: tokens.borderRadiusMedium,
		border: `1px solid ${tokens.colorNeutralStroke2}`,
		backgroundColor: tokens.colorNeutralBackground1,
		cursor: "pointer",
		transition: "all 0.1s ease",
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1Hover,
			border: `1px solid ${tokens.colorBrandStroke1}`,
		},
	},
	cardWeekend: {
		backgroundColor: tokens.colorNeutralBackground3,
	},
	cardHasEntry: {
		border: `1px solid ${tokens.colorBrandStroke1}`,
		backgroundColor: tokens.colorBrandBackground2,
	},
	cardDisabled: {
		cursor: "default",
		opacity: 0.7,
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1,
			border: `1px solid ${tokens.colorNeutralStroke2}`,
		},
	},
	left: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXXS,
	},
	dayName: {
		fontWeight: tokens.fontWeightSemibold,
		fontSize: tokens.fontSizeBase300,
	},
	date: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
	right: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
	},
	hours: {
		fontSize: tokens.fontSizeBase400,
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorBrandForeground1,
	},
	addIcon: {
		color: tokens.colorNeutralForeground3,
	},
	noteIcon: {
		color: tokens.colorNeutralForeground3,
	},
	timeRange: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
});

interface DayCardProps {
	calendarDay: CalendarDay;
	entry?: DayEntryResponse;
	canEdit: boolean;
	onClick: () => void;
}

const DayCard: React.FC<DayCardProps> = ({
	calendarDay,
	entry,
	canEdit,
	onClick,
}) => {
	const styles = useStyles();
	const hasEntry = !!entry;

	const cardClass = mergeClasses(
		styles.card,
		calendarDay.isWeekend && styles.cardWeekend,
		hasEntry && styles.cardHasEntry,
		!canEdit && styles.cardDisabled,
	);

	const formatDate = (date: Date) => {
		return date.toLocaleDateString("pl-PL", {
			day: "numeric",
			month: "long",
		});
	};

	return (
		<div
			className={cardClass}
			onClick={canEdit ? onClick : undefined}
			role="button"
			tabIndex={canEdit ? 0 : -1}
			onKeyDown={(e) => e.key === "Enter" && canEdit && onClick()}
		>
			<div className={styles.left}>
				<Text className={styles.dayName}>
					{getPolishDayNameFull(calendarDay.dayOfWeek)},{" "}
					{calendarDay.dayOfMonth}
				</Text>
				<Text className={styles.date}>{formatDate(calendarDay.date)}</Text>
				{entry && (
					<Text className={styles.timeRange}>
						{entry.startTime} - {entry.endTime}
					</Text>
				)}
			</div>
			<div className={styles.right}>
				{entry ? (
					<>
						<Text className={styles.hours}>
							{toTimeFormat(entry.workMinutes ?? 0)}
						</Text>
						{entry.notes && <NoteRegular className={styles.noteIcon} />}
					</>
				) : (
					canEdit && <AddRegular className={styles.addIcon} />
				)}
			</div>
		</div>
	);
};

export default DayCard;
