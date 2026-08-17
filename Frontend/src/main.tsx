import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { App } from "@/App";
import { env } from "@/config/env";
import "@/styles/global.css";

if (env.brandColor) {
  document.documentElement.style.setProperty("--brand-primary", env.brandColor);
}

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
