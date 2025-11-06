<script setup lang="ts">
interface Props {
  id: string;
  label: string;
  modelValue: boolean;
  description?: string;
  disabled?: boolean;
}

withDefaults(defineProps<Props>(), {
  description: '',
  disabled: false,
});

defineEmits<{
  'update:modelValue': [value: boolean];
}>();
</script>

<template>
  <div class="flex items-start gap-3">
    <div class="flex items-center h-5">
      <input
        :id="id"
        type="checkbox"
        :checked="modelValue"
        :disabled="disabled"
        class="w-4 h-4 border-gray-300 rounded text-blue-600 focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed dark:border-gray-600 dark:bg-gray-800 dark:focus:ring-blue-400"
        @change="$emit('update:modelValue', ($event.target as HTMLInputElement).checked)"
      />
    </div>

    <div class="flex flex-col gap-0.5">
      <label
        :for="id"
        class="text-sm font-medium text-gray-700 dark:text-gray-300 cursor-pointer"
      >
        {{ label }}
      </label>

      <p
        v-if="description"
        class="text-xs text-gray-500 dark:text-gray-400"
      >
        {{ description }}
      </p>
    </div>
  </div>
</template>
