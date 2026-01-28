import { Dropdown, Option } from "@fluentui/react-components";
import type React from "react";
import { useMemo } from "react";

function randomGid(): string {
	return crypto.randomUUID().replace(/-/g, "");
}

interface FakeSipAccount {
	gid: string;
	label: string;
}

const FAKE_SIP_ACCOUNTS: FakeSipAccount[] = [
	{ gid: randomGid(), label: "sip_user_101@pbx.local" },
	{ gid: randomGid(), label: "sip_user_102@pbx.local" },
	{ gid: randomGid(), label: "sip_reception@pbx.local" },
	{ gid: randomGid(), label: "sip_support@pbx.local" },
	{ gid: randomGid(), label: "sip_sales@pbx.local" },
	{ gid: randomGid(), label: "sip_manager@pbx.local" },
];

interface SipAccountPickerProps {
	value?: string | null;
	onChange: (gid: string) => void;
	disabled?: boolean;
}

const SipAccountPicker: React.FC<SipAccountPickerProps> = ({
	value,
	onChange,
	disabled,
}) => {
	const selectedAccount = useMemo(
		() => FAKE_SIP_ACCOUNTS.find((a) => a.gid === value),
		[value],
	);

	return (
		<Dropdown
			placeholder="Wybierz konto SIP..."
			value={selectedAccount?.label ?? ""}
			selectedOptions={value ? [value] : []}
			onOptionSelect={(_, data) => {
				if (data.optionValue) {
					onChange(data.optionValue);
				}
			}}
			disabled={disabled}
		>
			{FAKE_SIP_ACCOUNTS.map((account) => (
				<Option key={account.gid} value={account.gid} text={account.label}>
					{account.label}
				</Option>
			))}
		</Dropdown>
	);
};

export default SipAccountPicker;
