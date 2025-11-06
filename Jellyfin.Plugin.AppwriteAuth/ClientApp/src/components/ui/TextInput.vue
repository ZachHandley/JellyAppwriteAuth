<script setup lang="ts">
interface Props {
  id: string;
  label: string;
  modelValue: string | number;
  type?: string;
  placeholder?: string;
  description?: string;
  disabled?: boolean;
  required?: boolean;
}

withDefaults(defineProps<Props>(), {
  type: 'text',
  placeholder: '',
  description: '',
  disabled: false,
  required: false,
});

defineEmits<{
  'update:modelValue': [value: string | number];
}>();
</script>

<template>
  <div class="flex flex-col gap-1.5">
    <label
      :for="id"
      class="text-sm font-medium text-gray-700 dark:text-gray-300"
    >
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>

    <input
      :id="id"
      :type="type"
      :value="modelValue"
      :placeholder="placeholder"
      :disabled="disabled"
      :required="required"
      class="px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-gray-100 disabled:cursor-not-allowed dark:bg-gray-800 dark:border-gray-600 dark:text-gray-100 dark:placeholder-gray-400 dark:focus:ring-blue-400 dark:focus:border-blue-400 dark:disabled:bg-gray-900"
      @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    />

    <p
      v-if="description"
      class="text-xs text-gray-500 dark:text-gray-400"
    >
      {{ description }}
    </p>
  </div>
</template>
