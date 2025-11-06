import { ref } from 'vue';

export type ToastType = 'success' | 'error' | 'info';

export interface Toast {
  id: string;
  message: string;
  type: ToastType;
}

const toasts = ref<Toast[]>([]);
let toastIdCounter = 0;

export function useToast() {
  const showToast = (message: string, type: ToastType = 'info', duration = 3000) => {
    const id = `toast-${++toastIdCounter}`;
    const toast: Toast = { id, message, type };

    toasts.value.push(toast);

    if (duration > 0) {
      setTimeout(() => {
        removeToast(id);
      }, duration);
    }
  };

  const removeToast = (id: string) => {
    const index = toasts.value.findIndex((t) => t.id === id);
    if (index > -1) {
      toasts.value.splice(index, 1);
    }
  };

  const success = (message: string, duration?: number) => {
    showToast(message, 'success', duration);
  };

  const error = (message: string, duration?: number) => {
    showToast(message, 'error', duration);
  };

  const info = (message: string, duration?: number) => {
    showToast(message, 'info', duration);
  };

  return {
    toasts,
    showToast,
    removeToast,
    success,
    error,
    info,
  };
}
