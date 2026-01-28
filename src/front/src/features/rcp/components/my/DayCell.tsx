import {
	makeStyles,
	mergeClasses,
	Text,
	tokens,
} from "@fluentui/react-components";
import { NoteRegular } from "@fluentui/react-icons";
import type React from "react";
import type { DayEntryResponse } from "../../../../api/rcp/models";
import type { CalendarDay } from "../../utils";
import { toTimeFormat } from "../../utils";

const useStyles = makeStyles({
	cell: {
		display: "flex",
		flexDirection: "column",
		alignItems: "center",
		justifyContent: "flex-start",
		padding: tokens.spacingVerticalS,
		minHeight: "80px",
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
	cellDisabled: {
		cursor: "default",
		opacity: 0.5,
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1,
			border: `1px solid ${tokens.colorNeutralStroke2}`,
		},
	},
	cellWeekend: {
		backgroundColor: tokens.colorNeutralBackground3,
	},
	cellOutsideMonth: {
		opacity: 0.3,
		cursor: "default",
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1,
			border: `1px solid ${tokens.colorNeutralStroke2}`,
		},
	},
	cellHasEntry: {
		border: `1px solid ${tokens.colorBrandStroke1}`,
		backgroundColor: tokens.colorBrandBackground2,
	},
	dayNumber: {
		fontWeight: tokens.fontWeightSemibold,
		fontSize: tokens.fontSizeBase300,
	},
	hours: {
		fontSize: tokens.fontSizeBase400,
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorBrandForeground1,
		marginTop: tokens.spacingVerticalXS,
	},
	noteIcon: {
		marginTop: tokens.spacingVerticalXS,
		color: tokens.colorNeutralForeground3,
	},
});

interface DayCellProps {
	calendarDay: CalendarDay;
	entry?: DayEntryResponse;
	canEdit: boolean;
	onClick: () => void;
}

const DayCell: React.FC<DayCellProps> = ({
	calendarDay,
	entry,
	canEdit,
	onClick,
}) => {
	const styles = useStyles();

	const hasEntry = !!entry;
	const isDisabled = !calendarDay.isCurrentMonth || !canEdit;

	const cellClass = mergeClasses(
		styles.cell,
		calendarDay.isWeekend && styles.cellWeekend,
		!calendarDay.isCurrentMonth && styles.cellOutsideMonth,
		hasEntry && styles.cellHasEntry,
		isDisabled && styles.cellDisabled,
	);

	return (
		<div
			className={cellClass}
			onClick={isDisabled ? undefined : onClick}
			role="button"
			tabIndex={isDisabled ? -1 : 0}
			onKeyDown={(e) => e.key === "Enter" && !isDisabled && onClick()}
		>
			<Text className={styles.dayNumber}>{calendarDay.dayOfMonth}</Text>
			{entry && (
				<>
					<Text className={styles.hours}>
						{toTimeFormat(entry.workMinutes ?? 0)}
					</Text>
					{entry.notes && <NoteRegular className={styles.noteIcon} />}
				</>
			)}
		</div>
	);
};

export default DayCell;
