<script setup lang="ts">
import { ref } from 'vue';
import { useEmailActions } from '@/composables/useEmailActions';
import Button from '@/components/ui/Button.vue';
import TextInput from '@/components/ui/TextInput.vue';
import Card from '@/components/ui/Card.vue';

const emailActions = useEmailActions();

const inviteEmail = ref('');
const resetEmail = ref('');
const testEmail = ref('');

const handleSendInvite = async () => {
  if (!inviteEmail.value.trim()) return;

  try {
    await emailActions.sendInvite(inviteEmail.value);
    inviteEmail.value = '';
  } catch (error) {
    // Error is already handled by useEmailActions
    console.error('Failed to send invite:', error);
  }
};

const handleSendReset = async () => {
  if (!resetEmail.value.trim()) return;

  try {
    await emailActions.sendReset(resetEmail.value);
    resetEmail.value = '';
  } catch (error) {
    // Error is already handled by useEmailActions
    console.error('Failed to send reset:', error);
  }
};

const handleSendTest = async () => {
  if (!testEmail.value.trim()) return;

  try {
    await emailActions.sendTest(testEmail.value);
    testEmail.value = '';
  } catch (error) {
    // Error is already handled by useEmailActions
    console.error('Failed to send test:', error);
  }
};
</script>

<template>
  <Card title="Admin Actions">
    <div class="flex flex-col gap-6">
      <div class="bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-md px-4 py-3">
        <p class="text-sm text-yellow-800 dark:text-yellow-200">
          These actions will use the current email configuration from the form above.
          Make sure to save your configuration first if you've made changes.
        </p>
      </div>

      <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
        <h4 class="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-3">
          Send User Invitation
        </h4>
        <p class="text-xs text-gray-600 dark:text-gray-400 mb-4">
          Create a new user account and send them an invitation email with their credentials.
        </p>

        <div class="flex gap-2">
          <div class="flex-1">
            <TextInput
              id="invite-email"
              label=""
              v-model="inviteEmail"
              type="email"
              placeholder="user@example.com"
            />
          </div>
          <div class="flex items-end">
            <Button
              variant="primary"
              :loading="emailActions.loading.value"
              :disabled="!inviteEmail.trim()"
              @click="handleSendInvite"
            >
              Send Invite
            </Button>
          </div>
        </div>
      </div>

      <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
        <h4 class="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-3">
          Reset User Password
        </h4>
        <p class="text-xs text-gray-600 dark:text-gray-400 mb-4">
          Reset an existing user's password and send them an email with the new credentials.
        </p>

        <div class="flex gap-2">
          <div class="flex-1">
            <TextInput
              id="reset-email"
              label=""
              v-model="resetEmail"
              type="email"
              placeholder="user@example.com"
            />
          </div>
          <div class="flex items-end">
            <Button
              variant="secondary"
              :loading="emailActions.loading.value"
              :disabled="!resetEmail.trim()"
              @click="handleSendReset"
            >
              Reset Password
            </Button>
          </div>
        </div>
      </div>

      <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
        <h4 class="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-3">
          Send Test Email
        </h4>
        <p class="text-xs text-gray-600 dark:text-gray-400 mb-4">
          Send a test email to verify your email configuration is working correctly.
        </p>

        <div class="flex gap-2">
          <div class="flex-1">
            <TextInput
              id="test-email"
              label=""
              v-model="testEmail"
              type="email"
              placeholder="test@example.com"
            />
          </div>
          <div class="flex items-end">
            <Button
              variant="secondary"
              :loading="emailActions.loading.value"
              :disabled="!testEmail.trim()"
              @click="handleSendTest"
            >
              Send Test
            </Button>
          </div>
        </div>
      </div>
    </div>
  </Card>
</template>
