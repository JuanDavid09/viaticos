import { AppLogo } from "@/components/branding/AppLogo";

type LoginLogoProps = {
  variant?: "default" | "compact";
};

export function LoginLogo({ variant = "default" }: LoginLogoProps) {
  return <AppLogo variant={variant} />;
}
