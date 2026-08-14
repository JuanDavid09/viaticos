import { useState, type InputHTMLAttributes } from "react";
import { Eye, EyeOff } from "lucide-react";

type PasswordInputProps = Omit<InputHTMLAttributes<HTMLInputElement>, "type"> & {
  id: string;
};

export function PasswordInput({ id, className, ...props }: PasswordInputProps) {
  const [visible, setVisible] = useState(false);

  return (
    <div className={`password-field${className ? ` ${className}` : ""}`}>
      <input
        id={id}
        type={visible ? "text" : "password"}
        {...props}
      />
      <button
        type="button"
        className="password-toggle"
        onClick={() => setVisible((current) => !current)}
        aria-label={visible ? "Ocultar contraseña" : "Mostrar contraseña"}
        aria-pressed={visible}
        disabled={props.disabled}
        tabIndex={-1}
      >
        {visible ? <EyeOff size={18} /> : <Eye size={18} />}
      </button>
    </div>
  );
}
