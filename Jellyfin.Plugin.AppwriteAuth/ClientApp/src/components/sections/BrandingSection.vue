<script setup lang="ts">
import type { PluginConfiguration } from '@/types/config';
import TextInput from '@/components/ui/TextInput.vue';
import Card from '@/components/ui/Card.vue';

interface Props {
  modelValue: PluginConfiguration;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'update:modelValue': [value: PluginConfiguration];
}>();

const updateField = <K extends keyof PluginConfiguration>(
  field: K,
  value: PluginConfiguration[K]
) => {
  emit('update:modelValue', {
    ...props.modelValue,
    [field]: value,
  });
};
</script>

<template>
  <Card title="Branding Configuration">
    <div class="flex flex-col gap-4">
      <TextInput
        id="brand-name"
        label="Brand Name"
        :model-value="modelValue.BrandName"
        placeholder="Jellyfin"
        description="The name of your service (used in email templates)"
        @update:model-value="updateField('BrandName', $event as string)"
      />

      <TextInput
        id="logo-url"
        label="Logo URL or Path"
        :model-value="modelValue.LogoUrlOrPath"
        type="url"
        placeholder="https://example.com/logo.png"
        description="URL or file path to your logo (used in email templates)"
        @update:model-value="updateField('LogoUrlOrPath', $event as string)"
      />

      <div class="flex flex-col gap-1.5">
        <label
          for="brand-color"
          class="text-sm font-medium text-gray-700 dark:text-gray-300"
        >
          Brand Primary Color
        </label>

        <div class="flex gap-2 items-center">
          <input
            id="brand-color"
            type="color"
            :value="modelValue.BrandPrimaryColor"
            class="h-10 w-20 rounded border border-gray-300 dark:border-gray-600 cursor-pointer"
            @input="updateField('BrandPrimaryColor', ($event.target as HTMLInputElement).value)"
          />

          <TextInput
            id="brand-color-text"
            label=""
            :model-value="modelValue.BrandPrimaryColor"
            placeholder="#00a4dc"
            class="flex-1"
            @update:model-value="updateField('BrandPrimaryColor', $event as string)"
          />
        </div>

        <p class="text-xs text-gray-500 dark:text-gray-400">
          Primary brand color (used in email templates)
        </p>
      </div>

      <TextInput
        id="login-url"
        label="Login URL"
        :model-value="modelValue.LoginUrl"
        type="url"
        placeholder="https://jellyfin.example.com"
        description="URL users will use to login (included in email templates)"
        @update:model-value="updateField('LoginUrl', $event as string)"
      />
    </div>
  </Card>
</template>
