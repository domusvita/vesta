using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vesta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_DailyQuestions_DailyQuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Users_UserId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DailyQuestions_DailyQuestionId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyQuestions_Families_FamilyId",
                table: "DailyQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyQuestions_Questions_QuestionId",
                table: "DailyQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Users_CreatedBy",
                table: "Families");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyMembers_Families_FamilyId",
                table: "FamilyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyMembers_Users_UserId",
                table: "FamilyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_InviteCodes_Families_FamilyId",
                table: "InviteCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_InviteCodes_Users_CreatedBy",
                table: "InviteCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Families",
                table: "Families");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InviteCodes",
                table: "InviteCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FamilyMembers",
                table: "FamilyMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyQuestions",
                table: "DailyQuestions");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "questions");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "notifications");

            migrationBuilder.RenameTable(
                name: "Families",
                newName: "families");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "comments");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "answers");

            migrationBuilder.RenameTable(
                name: "InviteCodes",
                newName: "invite_codes");

            migrationBuilder.RenameTable(
                name: "FamilyMembers",
                newName: "family_members");

            migrationBuilder.RenameTable(
                name: "DailyQuestions",
                newName: "daily_questions");

            migrationBuilder.RenameColumn(
                name: "Auth0Id",
                table: "users",
                newName: "auth0id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "users",
                newName: "display_name");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "users",
                newName: "date_of_birth");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "users",
                newName: "avatar_url");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Auth0Id",
                table: "users",
                newName: "ix_users_auth0id");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "questions",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "questions",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "questions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "MinAge",
                table: "questions",
                newName: "min_age");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "questions",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "questions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "notifications",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "notifications",
                newName: "payload");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "notifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "notifications",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "notifications",
                newName: "is_read");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "notifications",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "notifications",
                newName: "ix_notifications_user_id_is_read");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "families",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "families",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "families",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "families",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "families",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Families_CreatedBy",
                table: "families",
                newName: "ix_families_created_by");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "comments",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "comments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "comments",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "comments",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "DailyQuestionId",
                table: "comments",
                newName: "daily_question_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "comments",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "comments",
                newName: "ix_comments_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_DailyQuestionId",
                table: "comments",
                newName: "ix_comments_daily_question_id");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "answers",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "answers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "answers",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "answers",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "DailyQuestionId",
                table: "answers",
                newName: "daily_question_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "answers",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_UserId",
                table: "answers",
                newName: "ix_answers_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_DailyQuestionId_UserId",
                table: "answers",
                newName: "ix_answers_daily_question_id_user_id");

            migrationBuilder.RenameColumn(
                name: "Uses",
                table: "invite_codes",
                newName: "uses");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "invite_codes",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "invite_codes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "MaxUses",
                table: "invite_codes",
                newName: "max_uses");

            migrationBuilder.RenameColumn(
                name: "IntendedRole",
                table: "invite_codes",
                newName: "intended_role");

            migrationBuilder.RenameColumn(
                name: "FamilyId",
                table: "invite_codes",
                newName: "family_id");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "invite_codes",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "invite_codes",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "invite_codes",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_InviteCodes_FamilyId",
                table: "invite_codes",
                newName: "ix_invite_codes_family_id");

            migrationBuilder.RenameIndex(
                name: "IX_InviteCodes_CreatedBy",
                table: "invite_codes",
                newName: "ix_invite_codes_created_by");

            migrationBuilder.RenameIndex(
                name: "IX_InviteCodes_Code",
                table: "invite_codes",
                newName: "ix_invite_codes_code");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "family_members",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "family_members",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "family_members",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "JoinedAt",
                table: "family_members",
                newName: "joined_at");

            migrationBuilder.RenameColumn(
                name: "FamilyId",
                table: "family_members",
                newName: "family_id");

            migrationBuilder.RenameIndex(
                name: "IX_FamilyMembers_UserId",
                table: "family_members",
                newName: "ix_family_members_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_FamilyMembers_FamilyId_UserId",
                table: "family_members",
                newName: "ix_family_members_family_id_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "daily_questions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "daily_questions",
                newName: "question_id");

            migrationBuilder.RenameColumn(
                name: "FamilyId",
                table: "daily_questions",
                newName: "family_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "daily_questions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AssignedDate",
                table: "daily_questions",
                newName: "assigned_date");

            migrationBuilder.RenameIndex(
                name: "IX_DailyQuestions_QuestionId",
                table: "daily_questions",
                newName: "ix_daily_questions_question_id");

            migrationBuilder.RenameIndex(
                name: "IX_DailyQuestions_FamilyId_AssignedDate",
                table: "daily_questions",
                newName: "ix_daily_questions_family_id_assigned_date");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_questions",
                table: "questions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_notifications",
                table: "notifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_families",
                table: "families",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_comments",
                table: "comments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_answers",
                table: "answers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_invite_codes",
                table: "invite_codes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_family_members",
                table: "family_members",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_daily_questions",
                table: "daily_questions",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_answers_daily_questions_daily_question_id",
                table: "answers",
                column: "daily_question_id",
                principalTable: "daily_questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_answers_users_user_id",
                table: "answers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_daily_questions_daily_question_id",
                table: "comments",
                column: "daily_question_id",
                principalTable: "daily_questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_user_id",
                table: "comments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_daily_questions_families_family_id",
                table: "daily_questions",
                column: "family_id",
                principalTable: "families",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_daily_questions_questions_question_id",
                table: "daily_questions",
                column: "question_id",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_families_users_created_by",
                table: "families",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_family_members_families_family_id",
                table: "family_members",
                column: "family_id",
                principalTable: "families",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_family_members_users_user_id",
                table: "family_members",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invite_codes_families_family_id",
                table: "invite_codes",
                column: "family_id",
                principalTable: "families",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_invite_codes_users_created_by",
                table: "invite_codes",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_notifications_users_user_id",
                table: "notifications",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_answers_daily_questions_daily_question_id",
                table: "answers");

            migrationBuilder.DropForeignKey(
                name: "fk_answers_users_user_id",
                table: "answers");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_daily_questions_daily_question_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_users_user_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_daily_questions_families_family_id",
                table: "daily_questions");

            migrationBuilder.DropForeignKey(
                name: "fk_daily_questions_questions_question_id",
                table: "daily_questions");

            migrationBuilder.DropForeignKey(
                name: "fk_families_users_created_by",
                table: "families");

            migrationBuilder.DropForeignKey(
                name: "fk_family_members_families_family_id",
                table: "family_members");

            migrationBuilder.DropForeignKey(
                name: "fk_family_members_users_user_id",
                table: "family_members");

            migrationBuilder.DropForeignKey(
                name: "fk_invite_codes_families_family_id",
                table: "invite_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_invite_codes_users_created_by",
                table: "invite_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_notifications_users_user_id",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_questions",
                table: "questions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_notifications",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "pk_families",
                table: "families");

            migrationBuilder.DropPrimaryKey(
                name: "pk_comments",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_answers",
                table: "answers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_invite_codes",
                table: "invite_codes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_family_members",
                table: "family_members");

            migrationBuilder.DropPrimaryKey(
                name: "pk_daily_questions",
                table: "daily_questions");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "questions",
                newName: "Questions");

            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "families",
                newName: "Families");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "Comments");

            migrationBuilder.RenameTable(
                name: "answers",
                newName: "Answers");

            migrationBuilder.RenameTable(
                name: "invite_codes",
                newName: "InviteCodes");

            migrationBuilder.RenameTable(
                name: "family_members",
                newName: "FamilyMembers");

            migrationBuilder.RenameTable(
                name: "daily_questions",
                newName: "DailyQuestions");

            migrationBuilder.RenameColumn(
                name: "auth0id",
                table: "Users",
                newName: "Auth0Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "Users",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "date_of_birth",
                table: "Users",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "avatar_url",
                table: "Users",
                newName: "AvatarUrl");

            migrationBuilder.RenameIndex(
                name: "ix_users_auth0id",
                table: "Users",
                newName: "IX_Users_Auth0Id");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Questions",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Questions",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Questions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "min_age",
                table: "Questions",
                newName: "MinAge");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Questions",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Questions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Notifications",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "payload",
                table: "Notifications",
                newName: "Payload");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Notifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Notifications",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "Notifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Notifications",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_notifications_user_id_is_read",
                table: "Notifications",
                newName: "IX_Notifications_UserId_IsRead");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Families",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Families",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Families",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "Families",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Families",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_families_created_by",
                table: "Families",
                newName: "IX_Families_CreatedBy");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Comments",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Comments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Comments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Comments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "daily_question_id",
                table: "Comments",
                newName: "DailyQuestionId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Comments",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_comments_user_id",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_comments_daily_question_id",
                table: "Comments",
                newName: "IX_Comments_DailyQuestionId");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Answers",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Answers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Answers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Answers",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "daily_question_id",
                table: "Answers",
                newName: "DailyQuestionId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Answers",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_answers_user_id",
                table: "Answers",
                newName: "IX_Answers_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_answers_daily_question_id_user_id",
                table: "Answers",
                newName: "IX_Answers_DailyQuestionId_UserId");

            migrationBuilder.RenameColumn(
                name: "uses",
                table: "InviteCodes",
                newName: "Uses");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "InviteCodes",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "InviteCodes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "max_uses",
                table: "InviteCodes",
                newName: "MaxUses");

            migrationBuilder.RenameColumn(
                name: "intended_role",
                table: "InviteCodes",
                newName: "IntendedRole");

            migrationBuilder.RenameColumn(
                name: "family_id",
                table: "InviteCodes",
                newName: "FamilyId");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "InviteCodes",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "InviteCodes",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "InviteCodes",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_invite_codes_family_id",
                table: "InviteCodes",
                newName: "IX_InviteCodes_FamilyId");

            migrationBuilder.RenameIndex(
                name: "ix_invite_codes_created_by",
                table: "InviteCodes",
                newName: "IX_InviteCodes_CreatedBy");

            migrationBuilder.RenameIndex(
                name: "ix_invite_codes_code",
                table: "InviteCodes",
                newName: "IX_InviteCodes_Code");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "FamilyMembers",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FamilyMembers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "FamilyMembers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "joined_at",
                table: "FamilyMembers",
                newName: "JoinedAt");

            migrationBuilder.RenameColumn(
                name: "family_id",
                table: "FamilyMembers",
                newName: "FamilyId");

            migrationBuilder.RenameIndex(
                name: "ix_family_members_user_id",
                table: "FamilyMembers",
                newName: "IX_FamilyMembers_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_family_members_family_id_user_id",
                table: "FamilyMembers",
                newName: "IX_FamilyMembers_FamilyId_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DailyQuestions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "question_id",
                table: "DailyQuestions",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "family_id",
                table: "DailyQuestions",
                newName: "FamilyId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "DailyQuestions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "assigned_date",
                table: "DailyQuestions",
                newName: "AssignedDate");

            migrationBuilder.RenameIndex(
                name: "ix_daily_questions_question_id",
                table: "DailyQuestions",
                newName: "IX_DailyQuestions_QuestionId");

            migrationBuilder.RenameIndex(
                name: "ix_daily_questions_family_id_assigned_date",
                table: "DailyQuestions",
                newName: "IX_DailyQuestions_FamilyId_AssignedDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Families",
                table: "Families",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InviteCodes",
                table: "InviteCodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FamilyMembers",
                table: "FamilyMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyQuestions",
                table: "DailyQuestions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_DailyQuestions_DailyQuestionId",
                table: "Answers",
                column: "DailyQuestionId",
                principalTable: "DailyQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Users_UserId",
                table: "Answers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_DailyQuestions_DailyQuestionId",
                table: "Comments",
                column: "DailyQuestionId",
                principalTable: "DailyQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyQuestions_Families_FamilyId",
                table: "DailyQuestions",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyQuestions_Questions_QuestionId",
                table: "DailyQuestions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Users_CreatedBy",
                table: "Families",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyMembers_Families_FamilyId",
                table: "FamilyMembers",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyMembers_Users_UserId",
                table: "FamilyMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InviteCodes_Families_FamilyId",
                table: "InviteCodes",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InviteCodes_Users_CreatedBy",
                table: "InviteCodes",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
