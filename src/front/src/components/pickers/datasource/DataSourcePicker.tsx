import type React from "react";
import type { DataSourceType } from "../../../api/datasource/models";
import TagPicker, { type TagPickerOption } from "../TagPicker";
import { useDataSourcePicker } from "./useDataSourcePicker";

export interface DataSourcePickerProps {
	dataSourceType: DataSourceType;
	label: string;
	value: string[];
	onChange: (ids: string[], options: TagPickerOption[]) => void;
	placeholder?: string;
	required?: boolean;
	error?: string;
	disabled?: boolean;
	maxSelectedItems?: number;
	contextFilters?: Record<string, string>;
	debounceMs?: number;
	initialSuggestionsLimit?: number;
	useGid?: boolean;
}

const DataSourcePicker: React.FC<DataSourcePickerProps> = ({
	dataSourceType,
	label,
	value,
	onChange,
	placeholder = "Wyszukaj...",
	required = false,
	error,
	disabled = false,
	maxSelectedItems,
	contextFilters,
	debounceMs = 300,
	initialSuggestionsLimit = 25,
	useGid = true,
}) => {
	const { loadOptions } = useDataSourcePicker({
		dataSourceType,
		contextFilters,
		useGid,
	});

	return (
		<TagPicker
			label={label}
			value={value}
			onChange={(ids, options) => onChange(ids, options)}
			loadOptions={loadOptions}
			placeholder={placeholder}
			required={required}
			error={error}
			disabled={disabled}
			maxSelectedItems={maxSelectedItems}
			debounceMs={debounceMs}
			initialSuggestionsLimit={initialSuggestionsLimit}
		/>
	);
};

export default DataSourcePicker;
