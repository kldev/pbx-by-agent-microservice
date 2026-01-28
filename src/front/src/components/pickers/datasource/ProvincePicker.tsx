import type React from "react";
import { DataSourceType } from "../../../api/datasource/models";
import type { TagPickerOption } from "../TagPicker";
import DataSourcePicker from "./DataSourcePicker";

export interface ProvincePickerProps {
	label?: string;
	value: string[];
	onChange: (gids: string[], options: TagPickerOption[]) => void;
	placeholder?: string;
	required?: boolean;
	error?: string;
	disabled?: boolean;
	maxSelectedItems?: number;
	countryGid?: string;
}

const ProvincePicker: React.FC<ProvincePickerProps> = ({
	label = "Województwo",
	value,
	onChange,
	placeholder = "Wyszukaj województwo...",
	required = false,
	error,
	disabled = false,
	maxSelectedItems = 1,
	countryGid,
}) => {
	const contextFilters = countryGid ? { countryGid } : undefined;

	return (
		<DataSourcePicker
			dataSourceType={DataSourceType.Provinces}
			label={label}
			value={value}
			onChange={onChange}
			placeholder={placeholder}
			required={required}
			error={error}
			disabled={disabled}
			maxSelectedItems={maxSelectedItems}
			contextFilters={contextFilters}
		/>
	);
};

export default ProvincePicker;
