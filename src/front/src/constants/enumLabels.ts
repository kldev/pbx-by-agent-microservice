import { RcpTimeEntryStatus } from "../api/rcp/models";

export interface EnumOption<T extends string = string> {
	value: T;
	label: string;
}

// Helper function to get label for enum value
export function getEnumLabel<T extends string>(
	options: EnumOption<T>[],
	value: T | null | undefined,
): string {
	if (value == null) return "-";
	const option = options.find((o) => o.value === value);
	return option?.label ?? value;
}

// RCP Time Entry Status
export const timeEntryStatusLabels: Record<RcpTimeEntryStatus, string> = {
	[RcpTimeEntryStatus.Draft]: "Wpisywanie",
	[RcpTimeEntryStatus.Submitted]: "Wysłane",
	[RcpTimeEntryStatus.Approved]: "Zatwierdzone",
	[RcpTimeEntryStatus.Correction]: "Do poprawy",
	[RcpTimeEntryStatus.Settlement]: "Rozliczenie",
};
