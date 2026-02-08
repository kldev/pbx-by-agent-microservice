import {
	Button,
	Checkbox,
	Dropdown,
	Option,
	Table,
	TableBody,
	TableCell,
	TableCellLayout,
	TableHeader,
	TableHeaderCell,
	TableRow,
	Text,
	makeStyles,
	tokens,
} from "@fluentui/react-components";
import { AddRegular, DeleteRegular } from "@fluentui/react-icons";
import type React from "react";
import { useCallback, useState } from "react";
import { DayOfWeek } from "../../../api/answerrule/models";
import type { TimeSlotDto } from "../../../api/answerrule/models";

const DAY_OF_WEEK_LABELS: Record<DayOfWeek, string> = {
	Monday: "Poniedziałek",
	Tuesday: "Wtorek",
	Wednesday: "Środa",
	Thursday: "Czwartek",
	Friday: "Piątek",
	Saturday: "Sobota",
	Sunday: "Niedziela",
};

const DAY_ORDER: DayOfWeek[] = [
	DayOfWeek.Monday,
	DayOfWeek.Tuesday,
	DayOfWeek.Wednesday,
	DayOfWeek.Thursday,
	DayOfWeek.Friday,
	DayOfWeek.Saturday,
	DayOfWeek.Sunday,
];

function generateTimeOptions(): string[] {
	const options: string[] = [];
	for (let h = 0; h < 24; h++) {
		for (let m = 0; m < 60; m += 15) {
			options.push(
				`${String(h).padStart(2, "0")}:${String(m).padStart(2, "0")}`,
			);
		}
	}
	return options;
}

const TIME_OPTIONS = generateTimeOptions();

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
	},
	addRow: {
		display: "flex",
		alignItems: "flex-end",
		gap: tokens.spacingHorizontalS,
		flexWrap: "wrap",
		padding: tokens.spacingVerticalS,
		backgroundColor: tokens.colorNeutralBackground2,
		borderRadius: tokens.borderRadiusMedium,
	},
	field: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalXXS,
	},
	fieldLabel: {
		fontSize: tokens.fontSizeBase200,
		color: tokens.colorNeutralForeground3,
	},
	error: {
		color: tokens.colorPaletteRedForeground1,
		fontSize: tokens.fontSizeBase200,
	},
	emptyState: {
		padding: tokens.spacingVerticalL,
		textAlign: "center",
		color: tokens.colorNeutralForeground3,
	},
});

interface TimeSlotsEditorProps {
	value: TimeSlotDto[];
	onChange: (slots: TimeSlotDto[]) => void;
	error?: string;
}

interface NewSlotState {
	dayOfWeek: DayOfWeek;
	startTime: string;
	endTime: string;
	isAllDay: boolean;
}

const defaultNewSlot: NewSlotState = {
	dayOfWeek: DayOfWeek.Monday,
	startTime: "08:00",
	endTime: "17:00",
	isAllDay: false,
};

const TimeSlotsEditor: React.FC<TimeSlotsEditorProps> = ({
	value,
	onChange,
	error,
}) => {
	const styles = useStyles();
	const [newSlot, setNewSlot] = useState<NewSlotState>(defaultNewSlot);
	const [addError, setAddError] = useState<string>("");

	const handleAdd = useCallback(() => {
		setAddError("");

		if (
			!newSlot.isAllDay &&
			newSlot.endTime &&
			newSlot.startTime &&
			newSlot.endTime <= newSlot.startTime
		) {
			setAddError("Czas zakończenia musi być większy niż czas rozpoczęcia");
			return;
		}

		const hasOverlap = value.some((slot) => {
			if (slot.dayOfWeek !== newSlot.dayOfWeek) return false;
			if (slot.isAllDay || newSlot.isAllDay) return true;
			const existStart = slot.startTime ?? "00:00";
			const existEnd = slot.endTime ?? "23:45";
			const newStart = newSlot.startTime;
			const newEnd = newSlot.endTime;
			return newStart < existEnd && newEnd > existStart;
		});

		if (hasOverlap) {
			setAddError("Przedziały czasowe nie mogą się nakładać w tym samym dniu");
			return;
		}

		const slot: TimeSlotDto = {
			dayOfWeek: newSlot.dayOfWeek,
			startTime: newSlot.isAllDay ? "00:00" : newSlot.startTime,
			endTime: newSlot.isAllDay ? "23:45" : newSlot.endTime,
			isAllDay: newSlot.isAllDay,
		};

		onChange([...value, slot]);
		setNewSlot(defaultNewSlot);
	}, [newSlot, value, onChange]);

	const handleRemove = useCallback(
		(index: number) => {
			onChange(value.filter((_, i) => i !== index));
		},
		[value, onChange],
	);

	const sortedSlots = [...value].sort((a, b) => {
		const dayA = DAY_ORDER.indexOf(a.dayOfWeek as DayOfWeek);
		const dayB = DAY_ORDER.indexOf(b.dayOfWeek as DayOfWeek);
		if (dayA !== dayB) return dayA - dayB;
		return (a.startTime ?? "").localeCompare(b.startTime ?? "");
	});

	return (
		<div className={styles.container}>
			{sortedSlots.length === 0 ? (
				<div className={styles.emptyState}>
					<Text>Brak przedziałów czasowych. Dodaj co najmniej jeden.</Text>
				</div>
			) : (
				<Table size="small">
					<TableHeader>
						<TableRow>
							<TableHeaderCell>Dzień</TableHeaderCell>
							<TableHeaderCell>Od</TableHeaderCell>
							<TableHeaderCell>Do</TableHeaderCell>
							<TableHeaderCell>Cały dzień</TableHeaderCell>
							<TableHeaderCell style={{ width: 50 }} />
						</TableRow>
					</TableHeader>
					<TableBody>
						{sortedSlots.map((slot, displayIdx) => {
							const originalIdx = value.indexOf(slot);
							return (
								<TableRow
									key={`${slot.dayOfWeek}-${slot.startTime}-${displayIdx}`}
								>
									<TableCell>
										<TableCellLayout>
											{DAY_OF_WEEK_LABELS[slot.dayOfWeek as DayOfWeek] ??
												slot.dayOfWeek}
										</TableCellLayout>
									</TableCell>
									<TableCell>{slot.isAllDay ? "-" : slot.startTime}</TableCell>
									<TableCell>{slot.isAllDay ? "-" : slot.endTime}</TableCell>
									<TableCell>{slot.isAllDay ? "Tak" : "Nie"}</TableCell>
									<TableCell>
										<Button
											appearance="subtle"
											icon={<DeleteRegular />}
											size="small"
											onClick={() => handleRemove(originalIdx)}
										/>
									</TableCell>
								</TableRow>
							);
						})}
					</TableBody>
				</Table>
			)}

			<div className={styles.addRow}>
				<div className={styles.field}>
					<Text className={styles.fieldLabel}>Dzień tygodnia</Text>
					<Dropdown
						value={DAY_OF_WEEK_LABELS[newSlot.dayOfWeek]}
						selectedOptions={[newSlot.dayOfWeek]}
						onOptionSelect={(_, data) => {
							if (data.optionValue) {
								setNewSlot((s) => ({
									...s,
									dayOfWeek: data.optionValue as DayOfWeek,
								}));
							}
						}}
						style={{ minWidth: 150 }}
					>
						{DAY_ORDER.map((day) => (
							<Option key={day} value={day} text={DAY_OF_WEEK_LABELS[day]}>
								{DAY_OF_WEEK_LABELS[day]}
							</Option>
						))}
					</Dropdown>
				</div>

				{!newSlot.isAllDay && (
					<>
						<div className={styles.field}>
							<Text className={styles.fieldLabel}>Od</Text>
							<Dropdown
								value={newSlot.startTime}
								selectedOptions={[newSlot.startTime]}
								onOptionSelect={(_, data) => {
									if (data.optionValue) {
										setNewSlot((s) => ({
											...s,
											startTime: data.optionValue!,
										}));
									}
								}}
								style={{ minWidth: 100 }}
							>
								{TIME_OPTIONS.map((t) => (
									<Option key={t} value={t} text={t}>
										{t}
									</Option>
								))}
							</Dropdown>
						</div>

						<div className={styles.field}>
							<Text className={styles.fieldLabel}>Do</Text>
							<Dropdown
								value={newSlot.endTime}
								selectedOptions={[newSlot.endTime]}
								onOptionSelect={(_, data) => {
									if (data.optionValue) {
										setNewSlot((s) => ({
											...s,
											endTime: data.optionValue!,
										}));
									}
								}}
								style={{ minWidth: 100 }}
							>
								{TIME_OPTIONS.map((t) => (
									<Option key={t} value={t} text={t}>
										{t}
									</Option>
								))}
							</Dropdown>
						</div>
					</>
				)}

				<div className={styles.field}>
					<Text className={styles.fieldLabel}>&nbsp;</Text>
					<Checkbox
						label="Cały dzień"
						checked={newSlot.isAllDay}
						onChange={(_, data) =>
							setNewSlot((s) => ({
								...s,
								isAllDay: !!data.checked,
							}))
						}
					/>
				</div>

				<div className={styles.field}>
					<Text className={styles.fieldLabel}>&nbsp;</Text>
					<Button
						appearance="primary"
						icon={<AddRegular />}
						onClick={handleAdd}
						size="medium"
					>
						Dodaj
					</Button>
				</div>
			</div>

			{addError && <Text className={styles.error}>{addError}</Text>}
			{error && <Text className={styles.error}>{error}</Text>}
		</div>
	);
};

export default TimeSlotsEditor;
